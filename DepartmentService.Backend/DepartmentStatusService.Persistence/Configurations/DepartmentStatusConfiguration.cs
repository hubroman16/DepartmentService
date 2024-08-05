using DepartmentStatusService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DepartmentStatusService.Persistence;

public class DepartmentStatusConfiguration: IEntityTypeConfiguration<DepartmentStatus>
{
    public void Configure(EntityTypeBuilder<DepartmentStatus> builder)
    {
        builder.HasKey(x => x.DepartmentId);
        builder.Property(b => b.Status)
            .IsRequired();
    }
}