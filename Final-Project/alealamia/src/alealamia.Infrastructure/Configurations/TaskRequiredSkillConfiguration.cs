using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Configurations
{
    internal class TaskRequiredSkillConfiguration : IEntityTypeConfiguration<TaskRequiredSkill>
    {
        public void Configure(EntityTypeBuilder<TaskRequiredSkill> builder)
        {
            builder.HasKey(x => new { x.TaskId , x.SkillId});
            builder.HasOne(x => x.Task)
                .WithMany(x => x.TaskRequiredSkills)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Skill)
                .WithMany(x => x.TaskRequiredSkills)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
