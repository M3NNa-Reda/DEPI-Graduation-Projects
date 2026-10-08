using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Infrastructure.Configurations
{
    internal class ProductOptionConfiguration : IEntityTypeConfiguration<ProductOption>
    {
        public void Configure(EntityTypeBuilder<ProductOption> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.HasOne(x => x.Product)
                    .WithMany(x => x.ProductOption)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
