using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Configurations
{
    internal class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(200);
            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasMaxLength(500);
        }
    }
}
