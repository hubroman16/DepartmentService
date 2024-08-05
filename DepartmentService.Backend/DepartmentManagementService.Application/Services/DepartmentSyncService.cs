using DepartmentManagementService.Domain.Abstractions;
using DepartmentManagementService.Domain.Models;
using Microsoft.Extensions.Logging;

namespace DepartmentManagementService.Application.Services;

public class DepartmentSyncService : IDepartmentSyncService
    {
        private readonly IDepartmentService _departmentService;
        private readonly IDepartmentStatusClientService _statusClientService;
        private readonly ILogger<DepartmentSyncService> _logger;

        public DepartmentSyncService(IDepartmentService departmentService, IDepartmentStatusClientService statusClientService, ILogger<DepartmentSyncService> logger)
        {
            _departmentService = departmentService;
            _statusClientService = statusClientService;
            _logger = logger;
        }

        public async Task SynchronizeDepartmentsAsync(IEnumerable<Department> fileDepartments)
        {
            var dbDepartments = await _departmentService.GetAllAsync();
            
            var dbDepartmentDict = dbDepartments.ToDictionary(d => d.Id);
            var fileDepartmentDict = fileDepartments.ToDictionary(d => d.Id);

            foreach (var fileDept in fileDepartments)
            {
                if (!dbDepartmentDict.ContainsKey(fileDept.Id))
                {
                    try
                    {
                        await _departmentService.AddAsync(fileDept);
                        await _departmentService.SaveChangesAsync();
                        
                        await _statusClientService.AddStatusAsync(fileDept.Id, "Заблокировано");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error adding department {DepartmentId}", fileDept.Id);
                        throw;
                    }
                }
                else
                {
                    var dbDept = dbDepartmentDict[fileDept.Id];
                    if (!DepartmentsAreEquivalent(dbDept, fileDept))
                    {
                        dbDept.Name = fileDept.Name;
                        dbDept.ParentId = fileDept.ParentId;

                        try
                        {
                            await _departmentService.UpdateAsync(dbDept);
                            await _departmentService.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error updating department {DepartmentId}", fileDept.Id);
                            throw;
                        }
                    }
                }
            }
            
            try
            {
                await _departmentService.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes to the database.");
                throw;
            }
        }

        private bool DepartmentsAreEquivalent(Department dbDept, Department fileDept)
        {
            return dbDept.Name == fileDept.Name && dbDept.ParentId == fileDept.ParentId;
        }
    }