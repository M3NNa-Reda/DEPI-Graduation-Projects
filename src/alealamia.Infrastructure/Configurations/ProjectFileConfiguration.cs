using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alealamia.Infrastructure.Configurations
{
    internal class ProjectFileConfiguration : IEntityTypeConfiguration<ProjectFile>
    {
        public void Configure(EntityTypeBuilder<ProjectFile> builder)
        {
            builder.HasKey(x => new { x.ProjectId, x.FileId });

            builder.Property(x => x.AddedAt)
                .HasColumnType("datetime2")
                .IsRequired();

            // builder.HasOne<Project>()
            //     .WithMany()
            //     .HasForeignKey(x => x.ProjectId);

            builder.HasOne(x => x.File)
                .WithMany(x => x.ProjectFiles)
                .HasForeignKey(x => x.FileId);
        }
    }
}