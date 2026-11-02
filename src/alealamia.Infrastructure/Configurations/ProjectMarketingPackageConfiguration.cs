using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alealamia.Infrastructure.Configurations;

public class ProjectMarketingPackageConfiguration : IEntityTypeConfiguration<ProjectMarketingPackage>
{
    public void Configure(EntityTypeBuilder<ProjectMarketingPackage> b)
    {
        b.ToTable("ProjectMarketingPackages");
        b.HasKey(x => x.Id);
        b.Property(x => x.PriceAtPurchase).IsRequired().HasPrecision(18, 2);
        b.Property(x => x.StartDate).IsRequired().HasColumnType("datetime2");
        b.Property(x => x.EndDate).IsRequired().HasColumnType("datetime2");
        b.Property(x => x.Status).IsRequired().HasConversion<int>();
        b.Property(x => x.CreatedAt).IsRequired().HasColumnType("datetime2");

        b.HasOne(x => x.Project)
            .WithMany(p => p.MarketingPackages)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.MarketingPackage)
            .WithMany(p => p.ProjectPurchases)
            .HasForeignKey(x => x.MarketingPackageId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.ProjectId);
        b.HasIndex(x => x.MarketingPackageId);
    }
}