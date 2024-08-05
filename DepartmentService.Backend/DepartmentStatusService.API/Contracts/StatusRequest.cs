namespace DepartmentStatusService.Contracts;

public class StatusRequest
{
    public Guid DepartmentId { get; set; }
    public string Status { get; set; }
}