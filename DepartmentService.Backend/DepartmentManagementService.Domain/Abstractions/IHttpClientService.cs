namespace DepartmentManagementService.Domain.Abstractions;

public interface IHttpClientService
{
    Task<string> GetAsync(string uri);
    Task PostAsync(string uri, string content);
    Task DeleteAsync(string uri);
}