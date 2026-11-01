using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Configurations
{
    internal class TrackSkillConfiguration : IEntityTypeConfiguration<TrackSkill>
    {
        public void Configure(EntityTypeBuilder<TrackSkill> builder)
        {
            builder.HasKey(x => new { x.TrackId, x.SkillId });
            builder.HasOne(x=>x.Skill)
                .WithMany(x=>x.TrackSkills)
                .HasForeignKey(x=>x.SkillId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Track)
                .WithMany(x => x.TrackSkills)
                .HasForeignKey(x => x.TrackId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
