using alealamia.Domain.Entities;
using alealamia.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FileEntity = alealamia.Domain.Entities.File;

namespace alealamia.Infrastructure.Configurations
{
    internal class FileConfiguration : IEntityTypeConfiguration<FileEntity>
    {
        public void Configure(EntityTypeBuilder<FileEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OriginalFileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.StoredFileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.FileUrl)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.ContentType)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.SizeInBytes)
                .IsRequired();

            builder.Property(x => x.UploadedByUserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(x => x.UploadedAt)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UploadedByUserId);
        }
    }
}