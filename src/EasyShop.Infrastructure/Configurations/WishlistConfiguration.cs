using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EasyShop.Infrastructure.Configurations
{
    internal class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.HasOne(x => x.ApplicationUser)
                .WithOne(x => x.Wishlist)
                .HasForeignKey<Wishlist>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
