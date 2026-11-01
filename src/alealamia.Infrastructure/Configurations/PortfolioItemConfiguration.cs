using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Configurations
{
    internal class PortfolioItemConfiguration : IEntityTypeConfiguration<PortfolioItem>
    {
        public void Configure(EntityTypeBuilder<PortfolioItem> builder)
        {
            builder.HasOne(x => x.FreelancerProfile)
                .WithMany(x => x.PortfolioItems)
                .HasForeignKey(x => x.FreelancerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();
            builder.Property(x => x.Description)
                .HasMaxLength(2000)
                .IsRequired();
            builder.Property(x => x.ProjectUrl)
                .HasMaxLength(500)
                .IsRequired(false);
            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

        }
    }
}
