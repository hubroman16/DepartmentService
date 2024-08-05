using DepartmentManagementService.Application.Contracts;
using DepartmentManagementService.Domain.Abstractions;
using Newtonsoft.Json;

namespace DepartmentManagementService.Application.Services
{
    public class DepartmentStatusClientService : IDepartmentStatusClientService
    {
        private readonly IHttpClientService _httpClientService;

        public DepartmentStatusClientService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<string> GetStatusAsync(Guid departmentId)
        {
            try
            {
                var responseContent = await _httpClientService.GetAsync($"api/DepartmentStatus/{departmentId}");
                var departmentStatus = JsonConvert.DeserializeObject<DepartmentStatusResponse>(responseContent);

                return departmentStatus.Status;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the status for department ID {departmentId}.", ex);
            }
        }

        public async Task UpdateStatusAsync(Guid departmentId, string status)
        {
            try
            {
                var requestContent = JsonConvert.SerializeObject(new DepartmentStatusRequest
                {
                    DepartmentId = departmentId,
                    Status = status
                });
                await _httpClientService.PostAsync("api/DepartmentStatus", requestContent);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating the status for department ID {departmentId}.", ex);
            }
        }

        public async Task AddStatusAsync(Guid departmentId, string status)
        {
            try
            {
                var requestContent = JsonConvert.SerializeObject(new DepartmentStatusRequest
                {
                    DepartmentId = departmentId,
                    Status = status
                });
                await _httpClientService.PostAsync("api/DepartmentStatus", requestContent);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while adding the status for department ID {departmentId}.", ex);
            }
        }

        public async Task DeleteStatusAsync(Guid departmentId)
        {
            try
            {
                await _httpClientService.DeleteAsync($"api/DepartmentStatus/{departmentId}");
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting the status for department ID {departmentId}.", ex);
            }
        }
    }
}
