namespace DepartmentStatusService.Domain.Models;

public class DepartmentStatus
{
    public Guid DepartmentId { get; set; }
    public string Status { get; set; }
    public DateTime LastUpdated { get; set; }
}