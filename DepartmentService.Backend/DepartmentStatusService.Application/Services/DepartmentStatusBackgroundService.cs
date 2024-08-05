using DepartmentStatusService.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace DepartmentStatusService.Application.Services
{
    public class DepartmentStatusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<DepartmentStatusBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(3);
        private readonly string[] _statuses = new[] { "Активно", "Заблокировано" };
        private int _currentStatusIndex = 0;

        public DepartmentStatusBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<DepartmentStatusBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DepartmentStatusBackgroundService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _currentStatusIndex = (_currentStatusIndex + 1) % _statuses.Length;
                    string newStatus = _statuses[_currentStatusIndex];

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var statusService = scope.ServiceProvider.GetRequiredService<IDepartmentStatusService>();
                        var allDepartments = await statusService.GetAllDepartmentsAsync();
                        foreach (var department in allDepartments)
                        {
                            await statusService.UpdateStatusAsync(department.DepartmentId, newStatus);
                            _logger.LogInformation($"Updated status for department {department.DepartmentId} to {newStatus}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating department statuses.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("DepartmentStatusBackgroundService is stopping.");
        }
    }
}
