using alealamia.Domain.Entities;
using alealamia.Domain.Enums;
using alealamia.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Configurations
{
    internal class TaskOfferConfiguration : IEntityTypeConfiguration<TaskOffer>
    {
        public void Configure(EntityTypeBuilder<TaskOffer> builder)
        {

            builder.HasOne(x => x.Task)
                .WithMany(x => x.TaskOffers)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x=>x.FreelancerProfile)
                .WithMany(x => x.TaskOffers)
                .HasForeignKey(x => x.FreelancerProfileId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();
            builder.Property(x => x.SentAt)
                .HasDefaultValueSql("GETUTCDATE()");
            builder.HasIndex(x => new { x.TaskId, x.FreelancerProfileId }).IsUnique();
        }
    }
}
