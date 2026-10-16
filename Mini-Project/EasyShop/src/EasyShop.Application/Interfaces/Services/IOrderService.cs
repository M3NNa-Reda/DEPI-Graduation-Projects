using EasyShop.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDetailsDto> CreateOrderAsync(int userId,CreateOrderRequestDto dto);
        Task<IEnumerable<OrderSummaryDto>> GetMyOrdersAsync(int userId);
        Task<OrderDetailsDto?> GetMyOrderByIdAsync(int userId,int orderId);
        Task CancelOrderAsync(int userId, int orderId);
        Task<IEnumerable<OrderSummaryDto>> GetRecentOrdersAsync(int page, int pageSize);
        Task<OrderDetailsDto?> GetOrderByIdForAdminAsync(int orderId);
        Task UpdateOrderStatusAsync(int adminId,int orderId,UpdateOrderStatusDto dto);
    }
}

