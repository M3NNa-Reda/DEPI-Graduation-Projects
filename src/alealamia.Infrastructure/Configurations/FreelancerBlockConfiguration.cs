using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Configurations
{
    internal class FreelancerBlockConfiguration : IEntityTypeConfiguration<FreelancerBlock>
    {
        public void Configure(EntityTypeBuilder<FreelancerBlock> builder)
        {
            builder.HasOne(x => x.Task)
                .WithMany(x => x.FreelancerBlocks)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x=>x.FreelancerProfile)
                .WithMany(x => x.FreelancerBlocks)
                .HasForeignKey(x => x.FreelancerProfileId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.StartedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.Reason).HasMaxLength(1000).IsRequired();

        }
    }
}
