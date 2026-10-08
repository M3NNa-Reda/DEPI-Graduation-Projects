using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyShop.Infrastructure.Configurations
{
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {

        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasPrecision(18, 2);


            builder.Property(p => p.PaymentMethod)
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(30);


            builder.HasIndex(p => p.OrderId)
                .IsUnique();

            builder.HasOne(p => p.Order)
                .WithMany()
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Transactions)
                .WithOne(pt => pt.Payment)
                .HasForeignKey(pt => pt.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
