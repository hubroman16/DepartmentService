using System.Text;
using DepartmentManagementService.Application.Contracts;
using DepartmentManagementService.Domain.Abstractions;
using Newtonsoft.Json;

namespace DepartmentManagementService.Application.Services
{
   public class DepartmentStatusClientService : IDepartmentStatusClientService
    {
        private readonly HttpClient _httpClient;

        public DepartmentStatusClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetStatusAsync(Guid departmentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/DepartmentStatus/{departmentId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error fetching status for department ID {departmentId}. Status code: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var departmentStatus = JsonConvert.DeserializeObject<DepartmentStatusResponse>(responseContent);

                return departmentStatus.Status;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the status for department ID {departmentId}.", ex);
            }
            catch (JsonException ex)
            {
                throw new ApplicationException($"Failed to parse the status response for department ID {departmentId}.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An unexpected error occurred while retrieving the status for department ID {departmentId}.", ex);
            }
        }

        public async Task UpdateStatusAsync(Guid departmentId, string status)
        {
            try
            {
                var requestContent = new StringContent(JsonConvert.SerializeObject(new DepartmentStatusRequest
                {
                    DepartmentId = departmentId,
                    Status = status
                }), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/DepartmentStatus", requestContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error updating status for department ID {departmentId}. Status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"An error occurred while updating the status for department ID {departmentId}.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An unexpected error occurred while updating the status for department ID {departmentId}.", ex);
            }
        }

        public async Task AddStatusAsync(Guid departmentId, string status)
        {
            try
            {
                var requestContent = new StringContent(JsonConvert.SerializeObject(new DepartmentStatusRequest
                {
                    DepartmentId = departmentId,
                    Status = status
                }), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/DepartmentStatus", requestContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error adding status for department ID {departmentId}. Status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"An error occurred while adding the status for department ID {departmentId}.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An unexpected error occurred while adding the status for department ID {departmentId}.", ex);
            }
        }

        public async Task DeleteStatusAsync(Guid departmentId)
        {
            try
            {
                var requestContent = new StringContent(JsonConvert.SerializeObject(new DepartmentStatusRequest
                {
                    DepartmentId = departmentId
                }), Encoding.UTF8, "application/json");

                var response = await _httpClient.DeleteAsync($"api/DepartmentStatus/{departmentId}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error deleting status for department ID {departmentId}. Status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"An error occurred while deleting the status for department ID {departmentId}.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An unexpected error occurred while deleting the status for department ID {departmentId}.", ex);
            }
        }
    }
}