using alealamia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alealamia.Infrastructure.Configurations
{
    internal class SupportMessageFileConfiguration : IEntityTypeConfiguration<SupportMessageFile>
    {
        public void Configure(EntityTypeBuilder<SupportMessageFile> builder)
        {
            builder.HasKey(x => new { x.SupportMessageId, x.FileId });

            builder.HasOne(x => x.SupportMessage)
                .WithMany(x => x.SupportMessageFiles)
                .HasForeignKey(x => x.SupportMessageId);

            builder.HasOne(x => x.File)
                .WithMany(x => x.SupportMessageFiles)
                .HasForeignKey(x => x.FileId);
        }
    }
}