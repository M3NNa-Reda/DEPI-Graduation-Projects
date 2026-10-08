using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace EasyShop.Infrastructure.Configurations
{
    internal class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(c => c.Code)
                .IsUnique();

            builder.Property(c => c.DiscountType)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(c => c.DiscountValue)
                .HasPrecision(18, 2);

            builder.Property(c => c.MinimumOrderAmount)
                .HasPrecision(18, 2);

            builder.Property(c => c.MaximumDiscountAmount)
                .HasPrecision(18, 2);


            /*the relationship between Coupon and Order is one-to-many, where one Coupon can be used in 
             many Orders. The foreign key in the Order entity is CouponId, which references the Id of
             the Coupon entity. The OnDelete(DeleteBehavior.Restrict) configuration ensures that a Coupon cannot be 
             deleted if it is associated with any Orders, preventing orphaned records in the Orders
              table.*/
            builder.HasMany(c => c.Orders)
                .WithOne(o => o.Coupon)
                .HasForeignKey(o => o.CouponId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Coupon_DiscountValue_Positive", "[DiscountValue] > 0");
                t.HasCheckConstraint("CK_Coupon_UsedCount_NonNegative", "[UsedCount] >= 0");
                t.HasCheckConstraint("CK_Coupon_ExpirationAfterStart", "[ExpirationDate] > [StartDate]");
                t.HasCheckConstraint("CK_Coupon_UsedCount_WithinLimit",
                    "[UsageLimit] IS NULL OR [UsedCount] <= [UsageLimit]");
                t.HasCheckConstraint("CK_Coupon_PercentageMax100",
                    "[DiscountType] <> 'Percentage' OR [DiscountValue] <= 100");
            });
        }
    }
}
