using EasyShop.Domain.Entities;
using EasyShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IOrderRepository:IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithItemsAsync(int orderId); 
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId); 
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetResentOrdersAsync(int page, int pageSize);


    }
}
