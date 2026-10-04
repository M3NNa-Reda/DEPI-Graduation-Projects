using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyShop.Infrastructure.Configurations
{
    internal class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
        {
            builder.Property(o => o.OldStatus)
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(o => o.NewStatus)
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(o => o.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(o => o.ChangedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(o => o.Order)
                .WithMany(o => o.OrderStatusHistories)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o=>o.ApplicationUser)
                .WithMany(o => o.OrderStatusHistories)
                .HasForeignKey(o => o.ChangedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
