using DepartmentStatusService.Domain.Models;

namespace DepartmentStatusService.Domain.Abstractions;

public interface IDepartmentStatusRepository
{
    Task<DepartmentStatus> GetStatusAsync(Guid departmentId);
    Task<IEnumerable<DepartmentStatus>> GetAllAsync();
    Task UpdateStatusAsync(DepartmentStatus status);
    Task AddStatusAsync(DepartmentStatus status);
    Task DeleteStatusAsync(Guid departmentId);
    Task SaveChangesAsync();
}