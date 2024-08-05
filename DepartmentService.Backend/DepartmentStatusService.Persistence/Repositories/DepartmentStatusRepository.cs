using DepartmentStatusService.Domain.Abstractions;
using DepartmentStatusService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DepartmentStatusService.Persistence.Repositories
{
    public class DepartmentStatusRepository : IDepartmentStatusRepository
    {
        private readonly DepartmentStatusDbContext _context;

        public DepartmentStatusRepository(DepartmentStatusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentStatus>> GetAllAsync()
        {
            try
            {
                return await _context.DepartmentStatuses.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving all department statuses.", ex);
            }
        }

        public async Task<DepartmentStatus> GetStatusAsync(Guid departmentId)
        {
            try
            {
                return await _context.DepartmentStatuses.FindAsync(departmentId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while retrieving the status for department with ID {departmentId}.", ex);
            }
        }

        public async Task AddStatusAsync(DepartmentStatus status)
        {
            if (status == null)
            {
                throw new ArgumentNullException(nameof(status), "The status cannot be null.");
            }

            try
            {
                await _context.DepartmentStatuses.AddAsync(status);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while adding the department status.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while adding the department status.", ex);
            }
        }

        public async Task DeleteStatusAsync(Guid departmentId)
        {
            try
            {
                var status = await _context.DepartmentStatuses.FindAsync(departmentId);
                if (status != null)
                {
                    _context.DepartmentStatuses.Remove(status);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"An error occurred while deleting the status for department with ID {departmentId}.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while deleting the status for department with ID {departmentId}.", ex);
            }
        }

        public async Task UpdateStatusAsync(DepartmentStatus status)
        {
            if (status == null)
            {
                throw new ArgumentNullException(nameof(status), "The status cannot be null.");
            }

            try
            {
                _context.DepartmentStatuses.Update(status);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while updating the department status.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while updating the department status.", ex);
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while saving changes.", ex);
            }
        }
    }
}
