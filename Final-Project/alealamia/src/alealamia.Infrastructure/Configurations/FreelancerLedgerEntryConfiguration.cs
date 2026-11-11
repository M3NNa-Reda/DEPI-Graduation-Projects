using alealamia.Domain.Entities;
using alealamia.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Configurations
{
    internal class FreelancerLedgerEntryConfiguration : IEntityTypeConfiguration<FreelancerLedgerEntry>
    {
        public void Configure(EntityTypeBuilder<FreelancerLedgerEntry> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.EntryType)
                .IsRequired();

            builder.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(t => t.Reason)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.HasOne(x=>x.FreelancerProfile)
                .WithMany(x=>x.FreelancerLedgerEntries)
                .HasForeignKey(t => t.FreelancerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x=>x.Task)
                .WithMany(x=>x.FreelancerLedgerEntries)
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                .WithMany(x=>x.FreelancerLedgerEntries)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
