using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyShop.Infrastructure.Configurations
{
    internal class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
    {
        public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
        {
            builder.HasOne(i => i.ProductVariant)
                .WithMany(i => i.InventoryTransactions)
                .HasForeignKey(i => i.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.TransactionType)
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(i => i.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
