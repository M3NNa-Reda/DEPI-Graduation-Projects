using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Configurations
{
    internal class PortfolioItemFileConfiguration : IEntityTypeConfiguration<PortfolioItemFile>
    {
        public void Configure(EntityTypeBuilder<PortfolioItemFile> builder)
        {
            builder.HasKey(x => new { x.PortfolioItemId, x.FileId });
            builder.HasOne(x => x.PortfolioItem)
               .WithMany(x => x.PortfolioItemFiles)
               .HasForeignKey(x => x.PortfolioItemId)
               .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.File)
                .WithMany(x => x.PortfolioItemFiles)
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
