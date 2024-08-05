using DepartmentManagementService.Domain.Abstractions;
using DepartmentManagementService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DepartmentManagementService.Persistence.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly DepartmentDbContext _context;

        public DepartmentRepository(DepartmentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            try
            {
                return await _context.Departments
                    .Include(d => d.Children)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving departments.", ex);
            }
        }

        public async Task<Department> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Departments
                    .Include(d => d.Children)
                    .FirstOrDefaultAsync(d => d.Id == id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while retrieving the department with ID {id}.", ex);
            }
        }

        public async Task AddAsync(Department department)
        {
            try
            {
                var existingDepartment = await _context.Departments
                    .AnyAsync(d => d.Name == department.Name);

                if (existingDepartment)
                {
                    throw new InvalidOperationException("A department with the same name already exists.");
                }

                await _context.Departments.AddAsync(department);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while adding the department.", ex);
            }
        }

        public async Task UpdateAsync(Department department)
        {
            try
            {
                var existingDepartment = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id != department.Id && d.Name == department.Name);

                if (existingDepartment != null)
                {
                    throw new InvalidOperationException("A department with the same name already exists.");
                }

                _context.Departments.Update(department);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while updating the department.", ex);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var department = await GetByIdAsync(id);
                if (department != null)
                {
                    _context.Departments.Remove(department);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while deleting the department with ID {id}.", ex);
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
