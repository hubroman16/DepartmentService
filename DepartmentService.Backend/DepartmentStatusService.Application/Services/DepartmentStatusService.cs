using DepartmentStatusService.Domain.Abstractions;
using DepartmentStatusService.Domain.Models;
using Microsoft.Extensions.Logging;

namespace DepartmentStatusService.Application.Services
{
    public class DepartmentStatusService : IDepartmentStatusService
    {
        private readonly IDepartmentStatusRepository _repository;
        private readonly ILogger<DepartmentStatusService> _logger;

        public DepartmentStatusService(IDepartmentStatusRepository repository, ILogger<DepartmentStatusService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<DepartmentStatus> GetStatusAsync(Guid departmentId)
        {
            try
            {
                var departmentStatus = await _repository.GetStatusAsync(departmentId);
                if (departmentStatus == null)
                {
                    _logger.LogWarning("Department status with ID {DepartmentId} not found.", departmentId);
                    throw new KeyNotFoundException($"Status for department with ID {departmentId} not found.");
                }
                return departmentStatus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving status for department ID {DepartmentId}.", departmentId);
                throw new ApplicationException($"An error occurred while retrieving status for department with ID {departmentId}.", ex);
            }
        }

        public async Task UpdateStatusAsync(Guid departmentId, string status)
        {
            try
            {
                var departmentStatus = await _repository.GetStatusAsync(departmentId);
                if (departmentStatus == null)
                {
                    _logger.LogWarning("Department status with ID {DepartmentId} not found for update.", departmentId);
                    throw new KeyNotFoundException($"Status for department with ID {departmentId} not found.");
                }

                departmentStatus.Status = status;
                departmentStatus.LastUpdated = DateTime.UtcNow;
                await _repository.UpdateStatusAsync(departmentStatus);

                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for department ID {DepartmentId}.", departmentId);
                throw new ApplicationException($"An error occurred while updating status for department with ID {departmentId}.", ex);
            }
        }
        
        public async Task AddStatusAsync(Guid departmentId, string status)
        {
            try
            {
                var existingStatus = await _repository.GetStatusAsync(departmentId);
                if (existingStatus != null)
                {
                    _logger.LogWarning("Status for department ID {DepartmentId} already exists. Use update instead.", departmentId);
                    throw new InvalidOperationException($"Status for department with ID {departmentId} already exists.");
                }

                var departmentStatus = new DepartmentStatus
                {
                    DepartmentId = departmentId,
                    Status = status,
                    LastUpdated = DateTime.UtcNow
                };
                await _repository.AddStatusAsync(departmentStatus);

                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding status for department ID {DepartmentId}.", departmentId);
                throw new ApplicationException($"An error occurred while adding status for department with ID {departmentId}.", ex);
            }
        }
        
        public async Task DeleteStatusAsync(Guid departmentId)
        {
            try
            {
                var departmentStatus = await _repository.GetStatusAsync(departmentId);
                if (departmentStatus == null)
                {
                    _logger.LogWarning("Department status with ID {DepartmentId} not found for deletion.", departmentId);
                    throw new KeyNotFoundException($"Status for department with ID {departmentId} not found.");
                }

                await _repository.DeleteStatusAsync(departmentId);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting status for department ID {DepartmentId}.", departmentId);
                throw new ApplicationException($"An error occurred while deleting status for department with ID {departmentId}.", ex);
            }
        }
        
        public async Task<IEnumerable<DepartmentStatus>> GetAllDepartmentsAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all department statuses.");
                throw new ApplicationException("An error occurred while retrieving all department statuses.", ex);
            }
        }
    }
}
