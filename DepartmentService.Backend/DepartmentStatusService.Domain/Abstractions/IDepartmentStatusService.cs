using DepartmentStatusService.Domain.Models;

namespace DepartmentStatusService.Domain.Abstractions;

public interface IDepartmentStatusService
{
    Task<DepartmentStatus> GetStatusAsync(Guid departmentId);
    Task UpdateStatusAsync(Guid departmentId, string status);
    Task AddStatusAsync(Guid departmentId, string status);
    Task DeleteStatusAsync(Guid departmentId);
    Task<IEnumerable<DepartmentStatus>> GetAllDepartmentsAsync(); 
}