using EasyShop.Application.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);
        Task AddToCartAsync(int userId, AddToCartDto dto);
        Task<bool> UpdateCartItemAsync(int userId, int productVariantId, int quantity);

        Task<bool> RemoveFromCartAsync(int userId, int productVariantId);
        Task ClearCartAsync(int userId);

    }
}
