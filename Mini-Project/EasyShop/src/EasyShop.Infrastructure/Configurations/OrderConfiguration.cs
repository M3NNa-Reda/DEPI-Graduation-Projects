using EasyShop.Domain.Entities;
using EasyShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Infrastructure.Configurations
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne(u => u.ApplicationUser)
                .WithMany(o => o.Orders)
                .HasForeignKey(ou => ou.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .HasDefaultValue(OrderStatus.Pending);

            builder.Property(o => o.Subtotal)
                .HasPrecision(18, 2);

            builder.Property(o => o.DiscountAmount)
                .HasPrecision(18, 2);

            builder.Property(o => o.ShippingFee)
                .HasPrecision(18, 2);

            builder.Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            builder.Property(o => o.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

        }
    }
}
