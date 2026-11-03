using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alealamia.Infrastructure.Configurations;

public class MarketingPackageConfiguration : IEntityTypeConfiguration<MarketingPackage>
{
    public void Configure(EntityTypeBuilder<MarketingPackage> b)
    {
        b.ToTable("MarketingPackages");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(150);
        b.Property(x => x.Description).HasMaxLength(1000);
        b.Property(x => x.DurationMonths).IsRequired();
        b.Property(x => x.DefaultPrice).IsRequired().HasPrecision(18, 2);
        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired().HasColumnType("datetime2");
        b.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}