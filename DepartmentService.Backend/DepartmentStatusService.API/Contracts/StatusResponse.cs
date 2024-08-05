namespace DepartmentStatusService.Contracts;

public class StatusResponse
{
    public Guid DepartmentId { get; set; }
    public string Status { get; set; }
    public DateTime LastUpdated { get; set; }
}