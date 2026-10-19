namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IOrderRepository:IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithItemsAsync(int orderId); 
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId); 
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int page, int pageSize);
        Task<Order?> GetOrderWithItemsByUserIdAsync(int userId, int orderId);
        Task<string?> GetShippingAddressByOrderIdAsync(int orderId);


    }
}
