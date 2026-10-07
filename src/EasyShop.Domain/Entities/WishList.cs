namespace EasyShop.Domain.Entities;

public class Wishlist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}