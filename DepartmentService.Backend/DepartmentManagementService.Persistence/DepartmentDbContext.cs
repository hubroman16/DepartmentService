using DepartmentManagementService.Domain.Models;
using DepartmentManagementService.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DepartmentManagementService.Persistence;

public class DepartmentDbContext(DbContextOptions<DepartmentDbContext> options) 
    : DbContext(options)
{
    public DbSet<Department> Departments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}


