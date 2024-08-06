using System.Text;
using DepartmentManagementService.Contracts;
using DepartmentManagementService.Domain.Abstractions;
using DepartmentManagementService.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DepartmentManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;
        private readonly IDepartmentStatusClientService _statusClient;
        private readonly IDepartmentSyncService _syncService;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(IDepartmentService service, IDepartmentStatusClientService statusClient, IDepartmentSyncService syncService, ILogger<DepartmentController> logger)
        {
            _service = service;
            _statusClient = statusClient;
            _syncService = syncService;
            _logger = logger;
        }

        // Синхронизация подразделений
        [HttpPost("sync")]
        public async Task<IActionResult> SyncDepartments(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            List<Department> fileDepartments;
            try
            {
                using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
                {
                    var fileContent = await reader.ReadToEndAsync();
                    fileDepartments = JsonConvert.DeserializeObject<List<Department>>(fileContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading file.");
                return StatusCode(500, "An error occurred while reading the file.");
            }

            try
            {
                await _syncService.SynchronizeDepartmentsAsync(fileDepartments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error synchronizing departments.");
                return StatusCode(500, "An error occurred while synchronizing departments.");
            }

            return Ok("Synchronization completed successfully.");
        }

        // Получение всех подразделений со статусами
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentResponse>>> GetAllDepartments([FromQuery] string name = null)
        {
            try
            {
                var departments = string.IsNullOrEmpty(name)
                    ? await _service.GetAllAsync()
                    : (await _service.GetAllAsync()).Where(d => d.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

                var departmentResponses = await Task.WhenAll(departments.Select(async d =>
                {
                    var response = MapToResponse(d);
                    try
                    {
                        response.Status = await _statusClient.GetStatusAsync(d.Id);
                        response.Children = await GetChildrenStatuses(d.Children);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error retrieving status for department {DepartmentId}", d.Id);
                        response.Status = "Error";
                        response.Children = await GetChildrenStatuses(d.Children);
                    }
                    return response;
                }));

                return Ok(departmentResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments.");
                return StatusCode(500, "An error occurred while retrieving departments.");
            }
        }

        // Получение статусов дочерних подразделений
        private async Task<List<DepartmentResponse>> GetChildrenStatuses(IEnumerable<Department> children)
        {
            var childResponses = new List<DepartmentResponse>();
            if (children != null)
            {
                foreach (var child in children)
                {
                    var childResponse = MapToResponse(child);
                    try
                    {
                        childResponse.Status = await _statusClient.GetStatusAsync(child.Id);
                        childResponse.Children = await GetChildrenStatuses(child.Children);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error retrieving status for child department {DepartmentId}", child.Id);
                        childResponse.Status = "Error";
                        childResponse.Children = await GetChildrenStatuses(child.Children);
                    }
                    childResponses.Add(childResponse);
                }
            }
            return childResponses;
        }

        // Получение подразделения по айди
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentResponse>> GetByID(Guid id)
        {
            var department = await _service.GetByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            var response = MapToResponse(department);
            try
            {
                response.Status = await _statusClient.GetStatusAsync(department.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving status for department {DepartmentId}", department.Id);
                response.Status = "Error";
            }

            return response;
        }

        // Добавление подразделения
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DepartmentRequest departmentRequest)
        {
            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = departmentRequest.Name,
                ParentId = departmentRequest.ParentId
            };
            var initialStatus = "Заблокировано";
            try
            {
                await _service.AddAsync(department);
                await _service.SaveChangesAsync();
                await _statusClient.AddStatusAsync(department.Id, initialStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding department {DepartmentId}", department.Id);
                return StatusCode(500, "Error adding department.");
            }

            var departmentResponse = MapToResponse(department);
            return CreatedAtAction(nameof(GetByID), new { id = department.Id }, departmentResponse);
        }

        // Изменение подразделения
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] DepartmentRequest departmentRequest)
        {
            var department = await _service.GetByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            department.Name = departmentRequest.Name;
            department.ParentId = departmentRequest.ParentId;

            await _service.UpdateAsync(department);

            return NoContent();
        }

        // Удаление подразделения
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                await _statusClient.DeleteStatusAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department {DepartmentId}", id);
                return StatusCode(500, "Error deleting department.");
            }
            return NoContent();
        }

        // Мэппинг ответа
        private DepartmentResponse MapToResponse(Department department)
        {
            return new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                ParentId = department.ParentId,
                Children = department.Children?.Select(MapToResponse).ToList() ?? new List<DepartmentResponse>()
            };
        }
    }
}