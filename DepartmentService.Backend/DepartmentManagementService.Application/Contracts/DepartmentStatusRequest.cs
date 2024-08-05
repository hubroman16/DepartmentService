namespace DepartmentManagementService.Application.Contracts;

public class DepartmentStatusRequest
{
    public Guid DepartmentId { get; set; }
    public string Status { get; set; }
}