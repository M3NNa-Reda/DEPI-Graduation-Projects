using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyShop.Infrastructure.Configurations;

public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> builder)
    {
        builder.HasKey(wi => wi.Id);

        builder.HasOne(wi => wi.Wishlist)
               .WithMany(w => w.WishlistItems)
               .HasForeignKey(wi => wi.WishlistId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wi => wi.Product)
               .WithMany()
               .HasForeignKey(wi => wi.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(wi => new
        {
            wi.WishlistId,
            wi.ProductId
        })
        .IsUnique();
    }
}