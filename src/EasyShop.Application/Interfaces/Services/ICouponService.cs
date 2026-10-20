namespace EasyShop.Application.Interfaces.Services
{
    public interface ICouponService
    {
        Task<IEnumerable<Coupon>> GetAllCouponsAsync();
        Task<Coupon?> GetCouponByIdAsync(int id);
        Task CreateCouponAsync(Coupon coupon);
        Task UpdateCouponAsync(Coupon coupon);
        Task<Coupon> ValidateCouponAsync(string code, int userId, decimal orderAmount);
        decimal CalculateDiscount(Coupon coupon, decimal orderAmount);
        bool CheckUsageLimit(Coupon coupon);
        bool CheckExpiration(Coupon coupon);
        Task ApplyCouponAsync(Coupon coupon, int userId, int orderId);
    }
}
