using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Configurations
{
    internal class TaskRevisionConfiguration : IEntityTypeConfiguration<TaskRevision>
    {
        public void Configure(EntityTypeBuilder<TaskRevision> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.RevisionNotes)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(t => t.NewDeadline)
                .IsRequired();

            builder.Property(t => t.Status)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.HasOne(x=>x.Task)
              .WithMany(x=>x.TaskRevisions)
              .HasForeignKey(t => t.TaskId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x=>x.TaskReview)
                .WithMany(x=>x.TaskRevisions)
                .HasForeignKey(t => t.TaskReviewId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
