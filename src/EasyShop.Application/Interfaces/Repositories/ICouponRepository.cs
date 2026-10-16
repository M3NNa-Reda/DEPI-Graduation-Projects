namespace EasyShop.Application.Interfaces.Repositories
{
    public interface ICouponRepository : IGenericRepository<Coupon>
    {
        Task<Coupon?> GetByCodeAsync(string code);

        Task<bool> TryIncrementUsageAsync(int couponId);
    }
}
