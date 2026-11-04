using alealamia.Domain.Entities;
using alealamia.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Configurations
{
    internal class TaskReassignmentConfiguration : IEntityTypeConfiguration<TaskReassignment>
    {
        public void Configure(EntityTypeBuilder<TaskReassignment> builder)
        {
            builder.HasOne(x=>x.Task)
                .WithMany(x=>x.TaskReassignments)
                .HasForeignKey(x=>x.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.PreviousFreelancer)
               .WithMany(x => x.PreviousReassignments)
               .HasForeignKey(x => x.PreviousFreelancerId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.NewFreelancer)
               .WithMany(x => x.NewReassignments)
               .HasForeignKey(x => x.NewFreelancerId)
               .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(2000);
            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.TaskReassignments)
                .HasForeignKey(x => x.ReassignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.ReassignedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
