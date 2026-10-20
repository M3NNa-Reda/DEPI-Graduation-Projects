namespace EasyShop.MVC.Controllers;

[Authorize(Roles = RoleNames.Admin)]
public class CouponsController : Controller
{
    private readonly ICouponService _couponService;

    public CouponsController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    // GET: /Coupons
    public async Task<IActionResult> Index()
    {
        var coupons = await _couponService.GetAllCouponsAsync();
        return View(coupons);
    }

    // GET: /Coupons/Create
    public IActionResult Create()
    {
        return View(new CouponFormViewModel());
    }

    // POST: /Coupons/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CouponFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _couponService.CreateCouponAsync(ToEntity(model));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }

        TempData["Success"] = $"Coupon \"{model.Code.Trim().ToUpperInvariant()}\" was created.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Coupons/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var coupon = await _couponService.GetCouponByIdAsync(id);
        if (coupon == null)
        {
            return NotFound();
        }

        return View(new CouponFormViewModel
        {
            Id = coupon.Id,
            Code = coupon.Code,
            DiscountType = coupon.DiscountType,
            DiscountValue = coupon.DiscountValue,
            MinimumOrderAmount = coupon.MinimumOrderAmount,
            MaximumDiscountAmount = coupon.MaximumDiscountAmount,
            UsageLimit = coupon.UsageLimit,
            StartDate = coupon.StartDate.Date,
            ExpirationDate = coupon.ExpirationDate.Date,
            IsActive = coupon.IsActive
        });
    }

    // POST: /Coupons/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CouponFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await _couponService.GetCouponByIdAsync(id) == null)
        {
            return NotFound();
        }

        try
        {
            await _couponService.UpdateCouponAsync(ToEntity(model));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }

        TempData["Success"] = $"Coupon \"{model.Code.Trim().ToUpperInvariant()}\" was updated.";
        return RedirectToAction(nameof(Index));
    }

    // The form works with dates; a coupon runs from the start of the first day to the end of the last day (UTC).
    private static Coupon ToEntity(CouponFormViewModel model) => new()
    {
        Id = model.Id,
        Code = model.Code,
        DiscountType = model.DiscountType,
        DiscountValue = model.DiscountValue,
        MinimumOrderAmount = model.MinimumOrderAmount,
        MaximumDiscountAmount = model.MaximumDiscountAmount,
        UsageLimit = model.UsageLimit,
        StartDate = DateTime.SpecifyKind(model.StartDate.Date, DateTimeKind.Utc),
        ExpirationDate = DateTime.SpecifyKind(model.ExpirationDate.Date.AddDays(1).AddSeconds(-1), DateTimeKind.Utc),
        IsActive = model.IsActive
    };
}
