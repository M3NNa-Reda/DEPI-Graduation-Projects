using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Infrastructure.Configurations
{
        internal class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
        {
            public void Configure(EntityTypeBuilder<ProductImage> builder)
            {
                builder.HasKey(i => i.Id);

                builder.Property(i => i.ImageUrl)
                        .IsRequired()
                        .HasMaxLength(500);

                builder.Property(i => i.IsPrimary)
                         .HasDefaultValue(false);

                builder.Property(i => i.DisplayOrder)
                         .HasDefaultValue(0);

                builder.HasOne(x => x.Product)
                        .WithMany(x => x.ProductImages)
                        .HasForeignKey(x => x.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);

                builder.HasIndex(x => x.ProductId)
                       .IsUnique()
                       .HasFilter("[IsPrimary] = 1");
            }
        }
}
