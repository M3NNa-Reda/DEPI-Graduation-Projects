using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Application.Services;
using EasyShop.Domain.Entities;
using EasyShop.Domain.Enums;

namespace EasyShop.Tests;

public class CouponServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ICouponRepository> _coupons = new();
    private readonly Mock<ICouponUsageRepository> _usages = new();
    private readonly CouponService _service;

    public CouponServiceTests()
    {
        _uow.SetupGet(u => u.Coupons).Returns(_coupons.Object);
        _uow.SetupGet(u => u.CouponUsages).Returns(_usages.Object);
        _service = new CouponService(_uow.Object);
    }

    private static Coupon ValidCoupon() => new()
    {
        Id = 1,
        Code = "SAVE10",
        DiscountType = DiscountType.Percentage,
        DiscountValue = 10,
        IsActive = true,
        StartDate = DateTime.UtcNow.AddDays(-1),
        ExpirationDate = DateTime.UtcNow.AddDays(1)
    };

    private void GivenCoupon(Coupon coupon) =>
        _coupons.Setup(r => r.GetByCodeAsync(coupon.Code)).ReturnsAsync(coupon);

    [Fact]
    public async Task Validate_rejects_an_unknown_code()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ValidateCouponAsync("NOPE", 1, 100m));
        Assert.Equal("Coupon code not found.", ex.Message);
    }

    [Fact]
    public async Task Validate_trims_and_upper_cases_the_code_before_looking_it_up()
    {
        GivenCoupon(ValidCoupon());

        var coupon = await _service.ValidateCouponAsync("  save10 ", 1, 100m);

        Assert.Equal("SAVE10", coupon.Code);
    }

    [Fact]
    public async Task Validate_rejects_an_inactive_coupon()
    {
        var coupon = ValidCoupon();
        coupon.IsActive = false;
        GivenCoupon(coupon);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ValidateCouponAsync("SAVE10", 1, 100m));
    }

    [Fact]
    public async Task Validate_rejects_a_coupon_that_has_not_started_yet()
    {
        var coupon = ValidCoupon();
        coupon.StartDate = DateTime.UtcNow.AddDays(1);
        coupon.ExpirationDate = DateTime.UtcNow.AddDays(2);
        GivenCoupon(coupon);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ValidateCouponAsync("SAVE10", 1, 100m));
    }

    [Fact]
    public async Task Validate_rejects_an_expired_coupon()
    {
        var coupon = ValidCoupon();
        coupon.StartDate = DateTime.UtcNow.AddDays(-5);
        coupon.ExpirationDate = DateTime.UtcNow.AddDays(-1);
        GivenCoupon(coupon);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ValidateCouponAsync("SAVE10", 1, 100m));
    }

    [Fact]
    public async Task Validate_rejects_a_coupon_that_reached_its_usage_limit()
    {
        var coupon = ValidCoupon();
        coupon.UsageLimit = 5;
        coupon.UsedCount = 5;
        GivenCoupon(coupon);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ValidateCouponAsync("SAVE10", 1, 100m));
        Assert.Equal("This coupon has reached its usage limit.", ex.Message);
    }

    [Fact]
    public async Task Validate_rejects_an_order_below_the_minimum_amount()
    {
        var coupon = ValidCoupon();
        coupon.MinimumOrderAmount = 500m;
        GivenCoupon(coupon);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ValidateCouponAsync("SAVE10", 1, 499.99m));
    }

    [Fact]
    public async Task Validate_rejects_a_customer_who_already_used_the_coupon()
    {
        GivenCoupon(ValidCoupon());
        _usages.Setup(r => r.HasUserUsedCouponAsync(1, 42)).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ValidateCouponAsync("SAVE10", 42, 100m));
        Assert.Equal("You have already used this coupon.", ex.Message);
    }

    [Fact]
    public async Task Validate_accepts_a_good_coupon()
    {
        var coupon = ValidCoupon();
        coupon.MinimumOrderAmount = 100m;
        coupon.UsageLimit = 10;
        GivenCoupon(coupon);

        var result = await _service.ValidateCouponAsync("SAVE10", 42, 100m);

        Assert.Same(coupon, result);
    }

    [Fact]
    public void CalculateDiscount_takes_a_percentage_of_the_order()
    {
        var discount = _service.CalculateDiscount(ValidCoupon(), 250m);
        Assert.Equal(25m, discount);
    }

    [Fact]
    public void CalculateDiscount_uses_the_fixed_amount()
    {
        var coupon = ValidCoupon();
        coupon.DiscountType = DiscountType.FixedAmount;
        coupon.DiscountValue = 40m;

        Assert.Equal(40m, _service.CalculateDiscount(coupon, 250m));
    }

    [Fact]
    public void CalculateDiscount_respects_the_maximum_discount()
    {
        var coupon = ValidCoupon();
        coupon.DiscountValue = 50;
        coupon.MaximumDiscountAmount = 30m;

        Assert.Equal(30m, _service.CalculateDiscount(coupon, 1000m));
    }

    [Fact]
    public void CalculateDiscount_never_exceeds_the_order_amount()
    {
        var coupon = ValidCoupon();
        coupon.DiscountType = DiscountType.FixedAmount;
        coupon.DiscountValue = 500m;

        Assert.Equal(80m, _service.CalculateDiscount(coupon, 80m));
    }

    [Fact]
    public async Task Create_rejects_a_percentage_above_one_hundred()
    {
        var coupon = ValidCoupon();
        coupon.DiscountValue = 120;

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateCouponAsync(coupon));
    }

    [Fact]
    public async Task Create_rejects_an_expiration_date_before_the_start_date()
    {
        var coupon = ValidCoupon();
        coupon.ExpirationDate = coupon.StartDate.AddHours(-1);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateCouponAsync(coupon));
    }

    [Fact]
    public async Task Create_rejects_a_code_that_already_exists()
    {
        _coupons.Setup(r => r.GetByCodeAsync("SAVE10")).ReturnsAsync(new Coupon { Id = 99, Code = "SAVE10" });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateCouponAsync(ValidCoupon()));
        Assert.Equal("A coupon with this code already exists.", ex.Message);
    }

    [Fact]
    public async Task Create_stores_the_code_normalised_and_starts_the_counter_at_zero()
    {
        var coupon = ValidCoupon();
        coupon.Code = " save10 ";
        coupon.UsedCount = 7;

        await _service.CreateCouponAsync(coupon);

        Assert.Equal("SAVE10", coupon.Code);
        Assert.Equal(0, coupon.UsedCount);
        _coupons.Verify(r => r.AddAsync(coupon), Times.Once);
        _uow.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task Apply_fails_when_the_usage_counter_cannot_be_incremented()
    {
        _coupons.Setup(r => r.TryIncrementUsageAsync(1)).ReturnsAsync(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ApplyCouponAsync(ValidCoupon(), 42, 10));

        _usages.Verify(r => r.AddAsync(It.IsAny<CouponUsage>()), Times.Never);
    }

    [Fact]
    public async Task Apply_records_who_used_the_coupon_on_which_order()
    {
        _coupons.Setup(r => r.TryIncrementUsageAsync(1)).ReturnsAsync(true);

        await _service.ApplyCouponAsync(ValidCoupon(), 42, 10);

        _usages.Verify(r => r.AddAsync(It.Is<CouponUsage>(u => u.CouponId == 1 && u.UserId == 42 && u.OrderId == 10)), Times.Once);
    }
}
