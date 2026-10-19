namespace EasyShop.Infrastructure.Repositories
{
    public class CouponUsageRepository : GenericRepository<CouponUsage>, ICouponUsageRepository
    {
        public CouponUsageRepository(AppDbContext context) : base(context) { }
        public async Task<bool> HasUserUsedCouponAsync(int couponId, int userId)
        {
            return await _context.CouponUsages
                .AsNoTracking()
                .AnyAsync(c => c.UserId == userId && c.CouponId == couponId);
        }
        
        public async Task<int> GetUserUsageCountAsync(int couponId, int userId)
        {
            return await _context.CouponUsages.AsNoTracking()
                .CountAsync(c => c.UserId == userId && c.CouponId == couponId);
        }

        public async Task<int> GetTotalUsageCountAsync(int couponId)
        {
            return await _context.CouponUsages.AsNoTracking()
                .CountAsync(c=> c.CouponId == couponId);
        }

    }
}
