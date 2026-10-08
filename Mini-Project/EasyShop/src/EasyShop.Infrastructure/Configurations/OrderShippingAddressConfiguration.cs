using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyShop.Infrastructure.Configurations
{
    internal class OrderShippingAddressConfiguration : IEntityTypeConfiguration<OrderShippingAddress>
    {
        public void Configure(EntityTypeBuilder<OrderShippingAddress> builder)
        {
            builder.HasOne(o => o.Order)
                .WithOne(o => o.OrderShippingAddress)
                .HasForeignKey<OrderShippingAddress>(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.OrderId)
                .IsUnique();

            builder.Property(o => o.FullName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(o => o.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.Country)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Area)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Street)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.BuildingNumber)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(o => o.ApartmentNumber)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(o => o.PostalCode)
                .HasMaxLength(20)
                .IsRequired(false);
        }
    }
}
