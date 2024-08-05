using DepartmentStatusService.Contracts;
using DepartmentStatusService.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using DepartmentStatusService.Domain.Models;

namespace DepartmentStatusService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentStatusController : ControllerBase
    {
        private readonly IDepartmentStatusService _statusService;
        private readonly ILogger<DepartmentStatusController> _logger;

        public DepartmentStatusController(IDepartmentStatusService statusService, ILogger<DepartmentStatusController> logger)
        {
            _statusService = statusService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<StatusResponse>> GetAllDepartments()
        {
            var departments = await _statusService.GetAllDepartmentsAsync();

            var departmentResponses = departments.Select(d => MapToResponse(d)).ToList();
            
            return departmentResponses;
        }
        
        [HttpGet("{departmentId}")]
        public async Task<ActionResult<StatusResponse>> GetStatus(Guid departmentId)
        {
            try
            {
                var status = await _statusService.GetStatusAsync(departmentId);
                if (status == null)
                {
                    return NotFound();
                }

                return Ok(new StatusResponse
                {
                    DepartmentId = status.DepartmentId,
                    Status = status.Status,
                    LastUpdated = status.LastUpdated
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving status for department {DepartmentId}", departmentId);
                return StatusCode(500, "An error occurred while retrieving the department status.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusRequest request)
        {
            if (request == null || request.DepartmentId == Guid.Empty || string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest("Invalid request data");
            }

            try
            {
                await _statusService.UpdateStatusAsync(request.DepartmentId, request.Status);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for department {DepartmentId}", request.DepartmentId);
                return StatusCode(500, "An error occurred while updating the department status.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddStatus([FromBody] StatusRequest request)
        {
            if (request == null || request.DepartmentId == Guid.Empty || string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest("Invalid request data");
            }

            try
            {
                await _statusService.AddStatusAsync(request.DepartmentId, request.Status);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding status for department {DepartmentId}", request.DepartmentId);
                return StatusCode(500, "An error occurred while adding the department status.");
            }
        }

        [HttpDelete("{departmentId}")]
        public async Task<IActionResult> DeleteStatus(Guid departmentId)
        {
            if (departmentId == Guid.Empty)
            {
                return BadRequest("Invalid request data");
            }

            try
            {
                await _statusService.DeleteStatusAsync(departmentId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting status for department {DepartmentId}", departmentId);
                return StatusCode(500, "An error occurred while deleting the department status.");
            }
        }
        
        private StatusResponse MapToResponse(DepartmentStatus department)
        {
            return new StatusResponse
            {
                DepartmentId = department.DepartmentId,
                Status = department.Status,
                LastUpdated = department.LastUpdated
            };
        }
    }
}
