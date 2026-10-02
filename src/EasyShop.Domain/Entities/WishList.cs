namespace EasyShop.Domain.Entities;

public class Wishlist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    //public ApplicationUser User { get; set; }
    public ICollection<WishlistItem> Items { get; set; }
}