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

            builder.HasOne(x => x.Project)
                .WithMany(x => x.ProjectFiles)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.File)
                .WithMany(x => x.ProjectFiles)
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}