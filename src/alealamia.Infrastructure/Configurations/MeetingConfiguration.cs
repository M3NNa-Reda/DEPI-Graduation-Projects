using alealamia.Domain.Entities;
using alealamia.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alealamia.Infrastructure.Configurations;

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> b)
    {
        b.ToTable("Meetings");
        b.HasKey(x => x.Id);
        b.Property(x => x.ProposedAt).IsRequired().HasColumnType("datetime2");
        b.Property(x => x.Status).IsRequired().HasConversion<int>();
        b.Property(x => x.AdminNotes).HasMaxLength(1000);
        b.Property(x => x.ReviewedByUserId).HasMaxLength(450);
        b.Property(x => x.ReviewedAt).HasColumnType("datetime2");
        b.Property(x => x.CreatedAt).IsRequired().HasColumnType("datetime2");

        b.HasOne(x => x.Project)
            .WithMany(p => p.Meetings)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.ClientProfile)
            .WithMany()
            .HasForeignKey(x => x.ClientProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.ProjectId);
        b.HasIndex(x => x.Status);
    }
}