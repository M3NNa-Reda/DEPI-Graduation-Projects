using EasyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Interfaces.Repositories
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetCartWithItemsByUserIdAsync(int userId);
        Task RemoveCartItems(IEnumerable<CartItem> cartItems);
    }
}
