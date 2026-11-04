using alealamia.Domain.Entities;
using alealamia.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace alealamia.Infrastructure.Configurations
{
    internal class SupportMessageConfiguration : IEntityTypeConfiguration<SupportMessage>
    {
        public void Configure(EntityTypeBuilder<SupportMessage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SupportTicketId)
                .IsRequired();

            builder.Property(x => x.SenderUserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(x => x.MessageText)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.SentAt)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.HasOne(x => x.SupportTicket)
                .WithMany(x => x.SupportMessages)
                .HasForeignKey(x => x.SupportTicketId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.SupportMessages)
                .HasForeignKey(x => x.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}