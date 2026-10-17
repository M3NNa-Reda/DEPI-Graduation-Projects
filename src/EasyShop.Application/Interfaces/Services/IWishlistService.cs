using EasyShop.Domain.Entities;

namespace EasyShop.Application.Interfaces.Services
{
    public interface IWishlistService
    {
        Task<Wishlist?> GetWishlistByIdAsync(int id);
        Task<Wishlist?> GetWishlistByUserIdAsync(int userId);
        Task<IEnumerable<Wishlist>> GetAllWishlistsAsync();
        Task AddWishlistAsync(Wishlist wishlist);
        void UpdateWishlist(Wishlist wishlist);
        void DeleteWishlist(Wishlist wishlist);
    }
}