using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alealamia.Infrastructure.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> b)
    {
        b.ToTable("Projects");
        b.HasKey(x => x.Id);
        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.Description).IsRequired();
        b.Property(x => x.HasCustomization).IsRequired();
        b.Property(x => x.Status).IsRequired().HasConversion<int>();
        b.Property(x => x.FinalPrice).HasPrecision(18, 2);
        b.Property(x => x.Currency).IsRequired().HasColumnType("char(3)");
        b.Property(x => x.CreatedAt).IsRequired().HasColumnType("datetime2");
        b.Property(x => x.UpdatedAt).HasColumnType("datetime2");

        b.HasOne(x => x.ClientProfile)
            .WithMany()
            .HasForeignKey(x => x.ClientProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.ClientProfileId);
        b.HasIndex(x => x.Status);
    }
}