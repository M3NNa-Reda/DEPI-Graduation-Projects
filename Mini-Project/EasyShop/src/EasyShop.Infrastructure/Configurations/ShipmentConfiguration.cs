using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace EasyShop.Infrastructure.Configurations
{
    internal  class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.TrackingNumber)
                .HasMaxLength(100);

            builder.Property(s => s.ShippingProvider)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.ShippingFee)
                .HasPrecision(18, 2);

            builder.Property(s => s.Status)
                .HasConversion<string>()
                .HasMaxLength(30);


            builder.HasIndex(s => s.TrackingNumber);

            builder.HasOne(s => s.Order)
                .WithMany(x=>x.Shipments)
                .HasForeignKey(s => s.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}