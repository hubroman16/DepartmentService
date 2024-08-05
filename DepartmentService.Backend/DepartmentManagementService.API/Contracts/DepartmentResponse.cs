namespace DepartmentManagementService.Contracts;

public class DepartmentResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public string Status { get; set; }
    public List<DepartmentResponse> Children { get; set; }
}