using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

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
        }
    }
}
