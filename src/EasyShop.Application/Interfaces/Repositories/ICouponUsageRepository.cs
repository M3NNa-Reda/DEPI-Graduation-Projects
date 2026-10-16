namespace EasyShop.Application.Interfaces.Repositories
{
    public interface ICouponUsageRepository:IGenericRepository<CouponUsage>
    {
        Task<bool> HasUserUsedCouponAsync(int couponId, int userId); // user used it?

        Task<int> GetUserUsageCountAsync(int couponId, int userId); // How many times used it?

        Task<int> GetTotalUsageCountAsync(int couponId); //all uses of coupon
    }
}

