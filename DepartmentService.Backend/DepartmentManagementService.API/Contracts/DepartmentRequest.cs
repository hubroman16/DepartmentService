namespace DepartmentManagementService.Contracts;

public class DepartmentRequest
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
}