using alealamia.Domain.Entities;
using alealamia.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Configurations
{
    internal class TaskStatusHistoryConfiguration : IEntityTypeConfiguration<TaskStatusHistory>
    {
        public void Configure(EntityTypeBuilder<TaskStatusHistory> builder)
        {
            builder.HasOne(x => x.Task)
                .WithMany(x => x.TaskStatusHistories)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.TaskStatusHistories)
                .HasForeignKey(x => x.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Notes)
                .HasMaxLength(2000)
                .IsRequired(false);
            builder.Property(x => x.ChangedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.OldStatus)
                .HasConversion<int?>();
            builder.Property(x => x.NewStatus)
               .IsRequired()
               .HasConversion<int>();

        }
    }
}
