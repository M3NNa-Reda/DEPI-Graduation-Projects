using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Infrastructure.Configurations
{
    internal class ProductVariantOptionValueConfiguration : IEntityTypeConfiguration<ProductVariantOptionValue>
    {
        public void Configure(EntityTypeBuilder<ProductVariantOptionValue> builder)
        {

            builder.HasKey(x => new
            {
                x.ProductVariantId,
                x.ProductOptionValueId
            });

            builder.HasOne(x => x.ProductVariant)
                   .WithMany(v => v.ProductVariantOptionValues)
                   .HasForeignKey(x => x.ProductVariantId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ProductOptionValue)
                   .WithMany(o => o.ProductVariantOptionValues)
                   .HasForeignKey(x => x.ProductOptionValueId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
