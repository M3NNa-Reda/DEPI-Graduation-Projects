using EasyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IUnitOfWork: IDisposable
    {
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        ICouponRepository Coupons { get; }
        ICouponUsageRepository CouponUsages { get; }
        IPaymentRepository Payments { get; }
        ICartRepository  Carts { get; }
        IReviewRepository Reviews { get; }
        IGenericRepository<Category> Categories { get; }
        IUserRepository Users { get; }
        IWishlistRepository Wishlists { get; }
        IProductVariantRepository ProductVariants { get; }
        IGenericRepository<UserAddress> UserAddresses { get; }

        Task<int> CompleteAsync();
    }
}
