namespace DepartmentManagementService.Domain.Abstractions;

public interface IDepartmentStatusClientService
{
    Task<string> GetStatusAsync(Guid departmentId);
    Task AddStatusAsync(Guid departmentId, string status);
    Task UpdateStatusAsync(Guid departmentId, string status);
    Task DeleteStatusAsync(Guid departmentId);
}