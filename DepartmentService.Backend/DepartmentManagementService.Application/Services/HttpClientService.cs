using System.Text;
using DepartmentManagementService.Domain.Abstractions;

namespace DepartmentManagementService.Application.Services;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;

    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetAsync(string uri)
    {
        var response = await _httpClient.GetAsync(uri);
            
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error fetching data from {uri}. Status code: {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync();
    }

    public async Task PostAsync(string uri, string content)
    {
        var requestContent = new StringContent(content, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(uri, requestContent);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error posting data to {uri}. Status code: {response.StatusCode}");
        }
    }

    public async Task DeleteAsync(string uri)
    {
        var response = await _httpClient.DeleteAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error deleting data from {uri}. Status code: {response.StatusCode}");
        }
    }
}