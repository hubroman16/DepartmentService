namespace DepartmentManagementService.Domain.Models;

public class Department
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public Department Parent { get; set; }
    public ICollection<Department> Children { get; set; }
}
