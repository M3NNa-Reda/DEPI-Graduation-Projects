using alealamia.Domain.Entities;
using alealamia.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Configurations
{
    internal class FreelancerSkillConfiguration : IEntityTypeConfiguration<FreelancerSkill>
    {
        public void Configure(EntityTypeBuilder<FreelancerSkill> builder)
        {
            builder.HasKey(x => new { x.FreelancerProfileId, x.SkillId });
            builder.HasOne(x => x.FreelancerProfile)
                .WithMany(x => x.FreelancerSkills)
                .HasForeignKey(x => x.FreelancerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Skill)
                .WithMany(x => x.FreelancerSkills)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.ProficiencyLevel)
                .HasConversion<int>();
            builder.Property(x => x.AddedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
