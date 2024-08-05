using DepartmentManagementService.Domain.Models;

namespace DepartmentManagementService.Domain.Abstractions;

public interface IDepartmentSyncService
{
    Task SynchronizeDepartmentsAsync(IEnumerable<Department> fileDepartments);
}