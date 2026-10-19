namespace EasyShop.Infrastructure.Repositories
{
    public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(AppDbContext context) : base(context) { }
        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            return await _context.Coupons
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == code);
        }
        public async Task<bool> TryIncrementUsageAsync(int couponId)
        {
            var rowsAffected = await _context.Coupons
                .Where(c => c.Id == couponId && (c.UsageLimit == null || c.UsedCount < c.UsageLimit))
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.UsedCount, c => c.UsedCount + 1));

            return rowsAffected > 0;
        }
    }
}
