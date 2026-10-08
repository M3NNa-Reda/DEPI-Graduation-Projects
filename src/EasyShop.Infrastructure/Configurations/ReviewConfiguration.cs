using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Infrastructure.Configurations
{
    internal class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasOne(v => v.ApplicationUser)
                .WithMany(u => u.Reviews)
                .HasForeignKey(vu => vu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.OrderItem)
                .WithOne(o => o.Review)
                .HasForeignKey<Review>(vo => vo.OrderItemId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(v => v.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(v => v.Comment)
                .IsRequired()
                .HasMaxLength(1000);
        }
    }
}
