using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Infrastructure.Persistence.Context;
using EasyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Infrastructure.Repositories
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IProductRepository Products { get; private set; }

        public IOrderRepository Orders { get; private set; }
        public ICouponRepository Coupons { get; private set; }
        public ICouponUsageRepository CouponUsages { get; private set; }
        public IPaymentRepository Payments { get; private set; }
        public ICartRepository Carts { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public IGenericRepository<Category> Categories { get; private set; }
        public IUserRepository Users { get; private set; }
        public IWishlistRepository Wishlists { get; private set; }
        public IProductVariantRepository ProductVariants { get; private set; }
        public IGenericRepository<UserAddress> UserAddresses { get; private set; }

        public UnitOfWork(AppDbContext context,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            ICouponRepository couponRepository ,
            ICouponUsageRepository couponUsageRepository,
            IPaymentRepository paymentRepository,
            ICartRepository cartRepository,
            IReviewRepository reviewRepository,
            IGenericRepository<Category> categoryRepository,
            IUserRepository userRepository,
            IWishlistRepository wishlistRepository,
            IProductVariantRepository productVariantRepository,
            IGenericRepository<UserAddress> userAddressRepository
            )
        {
            _context = context;
            Products = productRepository;
            Orders = orderRepository;
            Coupons = couponRepository;
            CouponUsages = couponUsageRepository;
            Payments = paymentRepository;
            Carts = cartRepository;
            Reviews = reviewRepository;
            Categories = categoryRepository;
            Users = userRepository;
            Wishlists = wishlistRepository;
            ProductVariants = productVariantRepository;
            UserAddresses = userAddressRepository;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
