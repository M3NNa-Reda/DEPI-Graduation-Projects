namespace EasyShop.Application.Services
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Coupon>> GetAllCouponsAsync()
        {
            var coupons = await _unitOfWork.Coupons.GetAllAsync();
            return coupons.OrderByDescending(c => c.CreatedAt);
        }

        public async Task<Coupon?> GetCouponByIdAsync(int id)
        {
            return await _unitOfWork.Coupons.GetByIdAsync(id);
        }

        public async Task CreateCouponAsync(Coupon coupon)
        {
            coupon.Code = NormalizeCode(coupon.Code);
            EnsureDefinitionIsValid(coupon, usedCount: 0);
            await EnsureCodeIsUniqueAsync(coupon.Code, excludedCouponId: null);

            coupon.UsedCount = 0;
            coupon.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Coupons.AddAsync(coupon);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateCouponAsync(Coupon updated)
        {
            var existing = await _unitOfWork.Coupons.GetByIdAsync(updated.Id)
                ?? throw new InvalidOperationException("Coupon not found.");

            updated.Code = NormalizeCode(updated.Code);
            EnsureDefinitionIsValid(updated, existing.UsedCount);
            await EnsureCodeIsUniqueAsync(updated.Code, excludedCouponId: existing.Id);

            // Copy onto the tracked entity so only the edited columns are written.
            // UsedCount is deliberately left alone: it is changed atomically by TryIncrementUsageAsync.
            existing.Code = updated.Code;
            existing.DiscountType = updated.DiscountType;
            existing.DiscountValue = updated.DiscountValue;
            existing.MinimumOrderAmount = updated.MinimumOrderAmount;
            existing.MaximumDiscountAmount = updated.MaximumDiscountAmount;
            existing.UsageLimit = updated.UsageLimit;
            existing.StartDate = updated.StartDate;
            existing.ExpirationDate = updated.ExpirationDate;
            existing.IsActive = updated.IsActive;

            await _unitOfWork.CompleteAsync();
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

        private static string NormalizeCode(string code) => (code ?? string.Empty).Trim().ToUpperInvariant();

        // Mirrors the CHECK constraints on the Coupons table so the admin gets a clear message instead of a database error.
        private static void EnsureDefinitionIsValid(Coupon coupon, int usedCount)
        {
            if (string.IsNullOrWhiteSpace(coupon.Code))
                throw new InvalidOperationException("Coupon code is required.");

            if (coupon.DiscountValue <= 0)
                throw new InvalidOperationException("Discount value must be greater than zero.");

            if (coupon.DiscountType == DiscountType.Percentage && coupon.DiscountValue > 100)
                throw new InvalidOperationException("A percentage discount cannot be more than 100.");

            if (coupon.ExpirationDate <= coupon.StartDate)
                throw new InvalidOperationException("The expiration date must be after the start date.");

            if (coupon.MinimumOrderAmount < 0)
                throw new InvalidOperationException("Minimum order amount cannot be negative.");

            if (coupon.MaximumDiscountAmount <= 0)
                throw new InvalidOperationException("Maximum discount amount must be greater than zero.");

            if (coupon.UsageLimit < 1)
                throw new InvalidOperationException("Usage limit must be at least 1.");

            if (coupon.UsageLimit < usedCount)
                throw new InvalidOperationException(
                    $"Usage limit cannot be lower than the {usedCount} times this coupon has already been used.");
        }

        private async Task EnsureCodeIsUniqueAsync(string code, int? excludedCouponId)
        {
            var sameCode = await _unitOfWork.Coupons.GetByCodeAsync(code);
            if (sameCode != null && sameCode.Id != excludedCouponId)
                throw new InvalidOperationException("A coupon with this code already exists.");
        }
    }
}