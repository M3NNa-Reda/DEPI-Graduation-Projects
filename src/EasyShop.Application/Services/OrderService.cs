using EasyShop.Application.DTOs.Order;
using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Application.Interfaces.Services;
using EasyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderDetailsDto> CreateOrderAsync(int userId, CreateOrderRequestDto dto)
        {
            // user login and authorized
            // cart and items
            //get product
            // product active and we have stock 
            // 
            var user =await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            var cart= await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                throw new KeyNotFoundException("Cart is empty or not found.");
            }
            var productVariantIds = cart.CartItems.Select(x => x.ProductVariantId).ToList();
            //var productVariants = await _unitOfWork.ProductsVariantsvar.FindByConditionAsync(pv => variantIds.Contains(pv.Id));

            //foreach (var productVariant in productVariants)
            //{

            //    if(productVariant==null||!productVariant.IsActive&&productVariant.)

            //}

        }

        public Task CancelOrderAsync(int userId, int orderId)
        {
            throw new NotImplementedException();
        }

       

        public Task<IEnumerable<OrderSummaryDto>> GetAllOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<OrderDetailsDto?> GetMyOrderByIdAsync(int userId, int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderSummaryDto>> GetMyOrdersAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDetailsDto?> GetOrderByIdForAdminAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOrderStatusAsync(int adminId, int orderId, UpdateOrderStatusDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
