using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alealamia.Infrastructure.Configurations
{
    internal class TaskSubmissionConfiguration : IEntityTypeConfiguration<TaskSubmission>
    {
        public void Configure(EntityTypeBuilder<TaskSubmission> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.SubmissionNotes)
                    .HasMaxLength(2000);

            builder.Property(t => t.Status)
                    .IsRequired();

            builder.HasOne(x=>x.Task)
                    .WithMany(x=>x.TaskSubmissions)
                    .HasForeignKey(t => t.TaskId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x=>x.FreelancerProfile)
                    .WithMany(x=>x.TaskSubmissions)
                    .HasForeignKey(t => t.FreelancerProfileId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
