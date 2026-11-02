using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alealamia.Infrastructure.Configurations;

public class ProjectServiceConfiguration : IEntityTypeConfiguration<ProjectService>
{
    public void Configure(EntityTypeBuilder<ProjectService> b)
    {
        b.ToTable("ProjectServices");
        b.HasKey(x => x.Id);
        b.Property(x => x.CustomizationDetails).HasMaxLength(2000);
        b.Property(x => x.CreatedAt).IsRequired().HasColumnType("datetime2");

        b.HasOne(x => x.Project)
            .WithMany(p => p.ProjectServices)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Service)
            .WithMany(s => s.ProjectServices)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.ProjectId, x.ServiceId }).IsUnique();
    }
}