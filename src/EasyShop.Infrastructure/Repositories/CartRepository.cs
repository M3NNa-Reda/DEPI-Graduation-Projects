using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Domain.Entities;
using EasyShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context) { }

        public async Task<Cart?> GetCartWithItemsByUserIdAsync(int userId)
        {
            return await _context.Carts
                 .AsNoTracking()
                 .Include(item => item.CartItems)
                 .FirstOrDefaultAsync(c => c.UserId == userId);
        }



        public async Task RemoveCartItems(IEnumerable<CartItem> cartItems)
        {
              _context.RemoveRange(cartItems);
        }
    }
}
