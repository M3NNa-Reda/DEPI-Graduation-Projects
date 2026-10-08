using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace EasyShop.Infrastructure.Configurations
{
    internal class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
    {
        public void Configure(EntityTypeBuilder<CouponUsage> builder)
        {
            builder.HasKey(cu => cu.Id);

            builder.HasOne(cu => cu.Coupon)
                .WithMany(c => c.CouponUsages)
                .HasForeignKey(cu => cu.CouponId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cu => cu.ApplicationUser)
                .WithMany(x => x.CouponUsages)
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cu => cu.Order)
                .WithOne(x=>x.CouponUsage)
                .HasForeignKey<CouponUsage>(cu => cu.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(cu => new { cu.CouponId, cu.OrderId })
                .IsUnique();

            builder.HasIndex(cu => cu.UserId);
        }
    }
}
