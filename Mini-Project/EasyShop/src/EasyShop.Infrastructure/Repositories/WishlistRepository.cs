using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Domain.Entities;
using EasyShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace EasyShop.Infrastructure.Repositories
{
    public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(AppDbContext context) : base(context) { }

        public async Task<Wishlist?> GetWishlistByUserIdAsync(int userId)
        {
            return await _context.Wishlists
                .AsNoTracking()
                .Include(w => w.WishlistItems)
                .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }
    }
}