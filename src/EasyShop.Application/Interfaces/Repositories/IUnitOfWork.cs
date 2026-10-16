namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        ICouponRepository Coupons { get; }
        ICouponUsageRepository CouponUsages { get; }
        IPaymentRepository Payments { get; }
        IPaymentTransactionRepository PaymentTransactions { get; }
        ICartRepository Carts { get; }
        IReviewRepository Reviews { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Brand> Brands { get; }
        IUserRepository Users { get; }
        IWishlistRepository Wishlists { get; }
        IProductVariantRepository ProductVariants { get; }
        IGenericRepository<UserAddress> UserAddresses { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<OrderStatusHistory> OrderStatusHistories { get; }

        Task<int> CompleteAsync();
    }
}