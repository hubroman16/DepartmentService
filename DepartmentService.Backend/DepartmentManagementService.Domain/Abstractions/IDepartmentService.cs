using DepartmentManagementService.Domain.Models;

namespace DepartmentManagementService.Domain.Abstractions;

public interface IDepartmentService
{
    Task<IEnumerable<Department>> GetAllAsync();
    Task<Department> GetByIdAsync(Guid id);
    Task AddAsync(Department department);
    Task UpdateAsync(Department department);
    Task DeleteAsync(Guid id);
    Task SaveChangesAsync();
}