using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alealamia.Infrastructure.Configurations;

public class ProjectMarketingPackageItemConfiguration : IEntityTypeConfiguration<ProjectMarketingPackageItem>
{
    public void Configure(EntityTypeBuilder<ProjectMarketingPackageItem> b)
    {
        b.ToTable("ProjectMarketingPackageItems");
        b.HasKey(x => x.Id);
        b.Property(x => x.NameSnapshot).IsRequired().HasMaxLength(150);
        b.Property(x => x.DescriptionSnapshot).HasMaxLength(500);
        b.Property(x => x.Quantity).IsRequired();

        b.HasOne(x => x.ProjectMarketingPackage)
            .WithMany(p => p.Items)
            .HasForeignKey(x => x.ProjectMarketingPackageId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.OriginalPackageItem)
            .WithMany()
            .HasForeignKey(x => x.OriginalPackageItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}