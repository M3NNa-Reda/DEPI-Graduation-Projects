using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alealamia.Infrastructure.Configurations;

public class MarketingPackageItemConfiguration : IEntityTypeConfiguration<MarketingPackageItem>
{
    public void Configure(EntityTypeBuilder<MarketingPackageItem> b)
    {
        b.ToTable("MarketingPackageItems");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(150);
        b.Property(x => x.Description).HasMaxLength(500);
        b.Property(x => x.DefaultQuantity).IsRequired();
        b.Property(x => x.IsActive).IsRequired();

        b.HasOne(x => x.MarketingPackage)
            .WithMany(p => p.Items)
            .HasForeignKey(x => x.MarketingPackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}