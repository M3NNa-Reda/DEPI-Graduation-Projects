using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Infrastructure.Persistence.Context;
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
        public UnitOfWork(AppDbContext context,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            ICouponRepository couponRepository ,
            ICouponUsageRepository couponUsageRepository,
            IPaymentRepository paymentRepository,
            ICartRepository cartRepository,
            IReviewRepository reviewRepository)
        {
            _context = context;
            Products = productRepository;
            Orders = orderRepository;
            Coupons = couponRepository;
            CouponUsages = couponUsageRepository;
            Payments = paymentRepository;
            Carts = cartRepository;
            Reviews = reviewRepository;
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
