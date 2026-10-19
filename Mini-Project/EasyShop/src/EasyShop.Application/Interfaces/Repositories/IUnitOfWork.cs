using EasyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        ICouponRepository Coupons { get; }
        ICouponUsageRepository CouponUsages { get; }
        IPaymentRepository Payments { get; }
        IGenericRepository<Shipment> Shipments { get; }
        IPaymentTransactionRepository PaymentTransactions { get; }
        ICartRepository Carts { get; }
        IReviewRepository Reviews { get; }
        IGenericRepository<Category> Categories { get; }
        IUserRepository Users { get; }
        IWishlistRepository Wishlists { get; }
        IProductVariantRepository ProductVariants { get; }
        IGenericRepository<UserAddress> UserAddresses { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<OrderStatusHistory> OrderStatusHistories { get; }
        IGenericRepository<Brand> Brands { get; }
        Task<int> CompleteAsync();
    }
}