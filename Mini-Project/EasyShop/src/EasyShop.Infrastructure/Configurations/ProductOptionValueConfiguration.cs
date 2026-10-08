using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Infrastructure.Configurations
{
    internal class ProductOptionValueConfiguration : IEntityTypeConfiguration<ProductOptionValue>
    {
        void IEntityTypeConfiguration<ProductOptionValue>.Configure(EntityTypeBuilder<ProductOptionValue> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Value)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.HasOne(x => x.ProductOption)
                    .WithMany(x => x.productOptionValues)
                    .HasForeignKey(x => x.ProductOptionId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
