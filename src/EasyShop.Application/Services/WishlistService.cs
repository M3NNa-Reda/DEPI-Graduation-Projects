using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Application.Interfaces.Services;
using EasyShop.Domain.Entities;

namespace EasyShop.Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WishlistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Wishlist?> GetWishlistByIdAsync(int id)
        {
            return await _unitOfWork.Wishlists.GetByIdAsync(id);
        }

        public async Task<Wishlist?> GetWishlistByUserIdAsync(int userId)
        {
            return await _unitOfWork.Wishlists.GetWishlistByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Wishlist>> GetAllWishlistsAsync()
        {
            return await _unitOfWork.Wishlists.GetAllAsync();
        }

        public async Task AddWishlistAsync(Wishlist wishlist)
        {
            await _unitOfWork.Wishlists.AddAsync(wishlist);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateWishlistAsync(Wishlist wishlist)
        {
            _unitOfWork.Wishlists.Update(wishlist);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteWishlistAsync(Wishlist wishlist)
        {
            _unitOfWork.Wishlists.Delete(wishlist);
            await _unitOfWork.CompleteAsync();
        }
    }
}