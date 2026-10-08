using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace EasyShop.Infrastructure.Configurations
{
    internal class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.TransactionReference)
                .HasMaxLength(150);

            builder.Property(pt => pt.Amount)
                .HasPrecision(18, 2);

            builder.Property(pt => pt.Status)
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(pt => pt.GatewayResponse)
                .HasMaxLength(2000);

            builder.HasOne(pt => pt.Payment)
                .WithMany(p => p.PaymentTransactions)
                .HasForeignKey(pt => pt.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}