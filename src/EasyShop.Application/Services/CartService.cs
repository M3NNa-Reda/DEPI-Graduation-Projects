using EasyShop.Application.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);

            if(cart == null)
                throw new ArgumentException("Cart Can't be null..");

            var cartItems = cart.CartItems.Select(
                item => new CartItemDto
                {
                    Id = item.Id,
                    ProductVariantId = item.ProductVariantId,
                    ProductName = item.ProductVariant.Product.Name,
                    UnitPrice = item.ProductVariant.Price,
                    Quantity = item.Quantity,
                    TotalPrice = item.ProductVariant.Price * item.Quantity
                }
                ).ToList();

            return new CartDto
            {
                Id = cart.Id,
                Items = cartItems,
                Subtotal = cartItems.Sum(i => i.TotalPrice)
            };

        }
        public async Task AddToCartAsync(int userId, AddToCartDto dto)
        {
            if(dto.Quantity <= 0)
                throw new ArgumentException("Quantity Can't be Negative");

            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);

            if(cart == null)
                throw new ArgumentException("Cart Not Found");

            var existingItem = cart.CartItems.
                FirstOrDefault(i => i.ProductVariantId == dto.ProductVariantId);

            if (existingItem != null)
                existingItem.Quantity += dto.Quantity;
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductVariantId = dto.ProductVariantId,
                    Quantity = dto.Quantity,
                    AddedAt = DateTime.UtcNow
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Carts.Update(cart);
            await _unitOfWork.CompleteAsync();

        }
        public async Task<bool> UpdateCartItemAsync(int userId, int productVariantId, int quantity)
        {
            if(quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero ");

            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);

            if (cart == null)
                throw new ArgumentException("Cart Not Found ");

            var item = cart.CartItems.FirstOrDefault(i => i.ProductVariantId == productVariantId);

            if(item == null)
                throw new ArgumentException("Cart Item Not Found");

            item.Quantity = quantity;
            cart.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Carts.Update(cart);
            await _unitOfWork.CompleteAsync();

            return true;

        }

        public async Task<bool> RemoveFromCartAsync(int userId, int productVariantId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);

            if (cart == null)
                throw new ArgumentException("Cart Not Found");

            var item = cart.CartItems.FirstOrDefault(i => i.ProductVariantId == productVariantId);

            if (item == null)
                throw new ArgumentException("Cart item Not Found");

            cart.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Carts.Update(cart);
            await _unitOfWork.CompleteAsync();

            return true;

        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);

            if (cart == null)
                throw new ArgumentException("Cart Not Found");

            cart.CartItems.Clear();
            cart.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Carts.Update(cart);
            await _unitOfWork.CompleteAsync();

        }

    }
}
