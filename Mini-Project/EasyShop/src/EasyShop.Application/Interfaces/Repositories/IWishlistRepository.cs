namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IWishlistRepository : IGenericRepository<Wishlist>
    {
        Task<Wishlist?> GetWishlistByUserIdAsync(int userId);
    }
}