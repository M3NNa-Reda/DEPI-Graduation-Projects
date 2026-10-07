using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Infrastructure.Configurations
{
    internal class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(b => b.Description)
                .HasMaxLength(2000)
                .IsRequired(false);

            builder.Property(b => b.LogoUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(b => b.IsActive)
                .HasDefaultValue(true);

            builder.Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(b => b.UpdatedAt)
                .IsRequired(false);

            builder.HasIndex(b => b.Name)
                .IsUnique();

            builder.HasIndex(b => b.IsActive);
        }
    }
}
