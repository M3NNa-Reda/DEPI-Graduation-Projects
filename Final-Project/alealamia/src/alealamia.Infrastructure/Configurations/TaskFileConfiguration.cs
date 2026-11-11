using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Configurations
{
    internal class TaskFileConfiguration : IEntityTypeConfiguration<TaskFile>
    {
        public void Configure(EntityTypeBuilder<TaskFile> builder)
        {
            builder.HasKey(x => new { x.TaskId, x.FileId });
            builder.HasOne(x => x.Task)
                .WithMany(x => x.TaskFiles)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.File)
                .WithMany(x => x.TaskFiles)
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.AddedAt)
                .HasDefaultValueSql("GETUTCDATE()");

        }
    }
}
