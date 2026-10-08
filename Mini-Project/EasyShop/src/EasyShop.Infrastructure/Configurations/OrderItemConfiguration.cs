using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Infrastructure.Configurations
{
    internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasOne(o => o.ProductVariant)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(op => op.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Property(x=>x.ProductName)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(o => o.Quantity)
                .IsRequired();

            builder.Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            builder.Property(o => o.UnitPrice)
                .HasPrecision(18, 2);
        }
    }
}
