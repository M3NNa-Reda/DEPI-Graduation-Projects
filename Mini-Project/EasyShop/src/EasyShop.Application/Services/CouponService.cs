namespace EasyShop.Application.Services
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Coupon> ValidateCouponAsync(string code, int userId, decimal orderAmount)
        {
            var coupon = await _unitOfWork.Coupons.GetByCodeAsync(code);

            if (coupon is null)
                throw new InvalidOperationException("Coupon code not found.");

            if (!CheckExpiration(coupon))
                throw new InvalidOperationException("This coupon is expired or not active yet.");

            if (!CheckUsageLimit(coupon))
                throw new InvalidOperationException("This coupon has reached its usage limit.");

            if (coupon.MinimumOrderAmount.HasValue && orderAmount < coupon.MinimumOrderAmount.Value)
                throw new InvalidOperationException(
                    $"This coupon requires a minimum order amount of {coupon.MinimumOrderAmount.Value:0.00}.");

            var alreadyUsed = await _unitOfWork.CouponUsages.HasUserUsedCouponAsync(coupon.Id, userId);
            if (alreadyUsed)
                throw new InvalidOperationException("You have already used this coupon.");

            return coupon;
        }
        public decimal CalculateDiscount(Coupon coupon, decimal orderAmount)
        {
            decimal discount = coupon.DiscountType switch
            {
                DiscountType.Percentage => orderAmount * (coupon.DiscountValue / 100m),
                DiscountType.FixedAmount => coupon.DiscountValue,
                _ => 0m
            };

            if (coupon.MaximumDiscountAmount.HasValue)
                discount = Math.Min(discount, coupon.MaximumDiscountAmount.Value);
            return Math.Min(discount, orderAmount);
        }
        public bool CheckUsageLimit(Coupon coupon)
        {
            return !coupon.UsageLimit.HasValue || coupon.UsedCount < coupon.UsageLimit.Value;
        }
        public bool CheckExpiration(Coupon coupon)
        {
            var now = DateTime.UtcNow;
            return coupon.IsActive && now >= coupon.StartDate && now <= coupon.ExpirationDate;
        }
        public async Task ApplyCouponAsync(Coupon coupon, int userId, int orderId)
        {
            if (!CheckExpiration(coupon))
                throw new InvalidOperationException("This coupon is expired or not active yet.");
            var incremented = await _unitOfWork.Coupons.TryIncrementUsageAsync(coupon.Id);
            if (!incremented)
                throw new InvalidOperationException("This coupon has reached its usage limit.");

            var usage = new CouponUsage
            {
                CouponId = coupon.Id,
                UserId = userId,
                OrderId = orderId,
                UsedAt = DateTime.UtcNow
            };
            await _unitOfWork.CouponUsages.AddAsync(usage);
            await _unitOfWork.CompleteAsync();
        }
    }
}