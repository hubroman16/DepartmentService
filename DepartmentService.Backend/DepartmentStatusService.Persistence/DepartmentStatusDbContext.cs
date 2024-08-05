using DepartmentStatusService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DepartmentStatusService.Persistence;

public class DepartmentStatusDbContext(DbContextOptions<DepartmentStatusDbContext> options) 
    : DbContext(options)
{
    public DbSet<DepartmentStatus> DepartmentStatuses { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DepartmentStatusConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}