using alealamia.Domain.Entities;
using alealamia.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alealamia.Infrastructure.Configurations;

public class ProjectStatusHistoryConfiguration : IEntityTypeConfiguration<ProjectStatusHistory>
{
    public void Configure(EntityTypeBuilder<ProjectStatusHistory> b)
    {
        b.ToTable("ProjectStatusHistories");
        b.HasKey(x => x.Id);
        b.Property(x => x.OldStatus).HasConversion<int?>();
        b.Property(x => x.NewStatus).IsRequired().HasConversion<int>();
        b.Property(x => x.ChangedByUserId).IsRequired().HasMaxLength(450);
        b.Property(x => x.Notes).HasMaxLength(500);
        b.Property(x => x.ChangedAt).IsRequired().HasColumnType("datetime2");

        b.HasOne(x => x.Project)
            .WithMany(p => p.StatusHistory)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.ProjectId, x.ChangedAt });
    }
}