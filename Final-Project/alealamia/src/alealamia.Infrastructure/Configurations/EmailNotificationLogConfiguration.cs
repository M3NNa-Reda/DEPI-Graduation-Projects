using alealamia.Domain.Entities;
using alealamia.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alealamia.Infrastructure.Configurations
{
    internal class EmailNotificationLogConfiguration : IEntityTypeConfiguration<EmailNotificationLog>
    {
        public void Configure(EntityTypeBuilder<EmailNotificationLog> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .HasMaxLength(450)
                .IsRequired(false);

            builder.Property(x => x.RecipientEmail)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.Subject)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(x => x.NotificationType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000)
                .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(x => x.SentAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired(false);
        }
    }
}