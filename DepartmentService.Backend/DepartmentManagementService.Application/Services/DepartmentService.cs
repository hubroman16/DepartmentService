using DepartmentManagementService.Domain.Abstractions;
using DepartmentManagementService.Domain.Models;
using Microsoft.Extensions.Logging;

namespace DepartmentManagementService.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(IDepartmentRepository repository, ILogger<DepartmentService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all departments.");
                throw new ApplicationException("An error occurred while fetching all departments.", ex);
            }
        }

        public async Task<Department> GetByIdAsync(Guid id)
        {
            try
            {
                var department = await _repository.GetByIdAsync(id);
                if (department == null)
                {
                    throw new KeyNotFoundException($"Department with ID {id} not found.");
                }
                return department;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Department with ID {Id} not found.", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the department with ID {Id}.", id);
                throw new ApplicationException($"An error occurred while fetching the department with ID {id}.", ex);
            }
        }

        public async Task AddAsync(Department department)
        {
            try
            {
                await _repository.AddAsync(department);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the department.");
                throw new ApplicationException("An error occurred while adding the department.", ex);
            }
        }

        public async Task UpdateAsync(Department department)
        {
            try
            {
                await _repository.UpdateAsync(department);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the department.");
                throw new ApplicationException("An error occurred while updating the department.", ex);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the department with ID {Id}.", id);
                throw new ApplicationException($"An error occurred while deleting the department with ID {id}.", ex);
            }
        }
        
        public async Task SaveChangesAsync()
        {
            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving changes.");
                throw new ApplicationException("An error occurred while saving changes.", ex);
            }
        }
    }
}