using EasyShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyShop.Infrastructure.Persistence.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        //Configurations
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<OrderShippingAddress> OrderShippingAddresses { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
