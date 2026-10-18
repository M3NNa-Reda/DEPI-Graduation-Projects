namespace EasyShop.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context):base(context) { }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o=>o.OrderItems)
                .Where(o => o.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Order?> GetOrderWithItemsAsync(int orderId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(it => it.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int page, int pageSize)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Order?> GetOrderWithItemsByUserIdAsync(int userId, int orderId)
        {
            return await _context.Orders
                .Include(it => it.OrderItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == orderId);
        }
        public async Task<string?> GetShippingAddressByOrderIdAsync(int orderId)
        {
            return await _context.Set<OrderShippingAddress>()
                .Where(a => a.OrderId == orderId)
                .Select(a => $"{a.FullName} - {a.PhoneNumber}, {a.Street}, {a.Area}, {a.City}, {a.Country}")
                .FirstOrDefaultAsync();
        }
    }
}
