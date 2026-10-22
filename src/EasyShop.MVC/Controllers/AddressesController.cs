using EasyShop.MVC.ViewModels;

namespace EasyShop.MVC.Controllers;

[Authorize]
public class AddressesController : Controller
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    // GET: /Addresses
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var addresses = await _addressService.GetUserAddressesAsync(User.GetUserId());
        return View(addresses);
    }

    // GET: /Addresses/Create
    [HttpGet]
    public IActionResult Create(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new AddressFormViewModel());
    }

    // POST: /Addresses/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddressFormViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        int userId = User.GetUserId();
        var existing = await _addressService.GetUserAddressesAsync(userId);

        await _addressService.AddAddressAsync(new UserAddress
        {
            UserId = userId,
            Country = model.Country.Trim(),
            City = model.City.Trim(),
            Area = model.Area.Trim(),
            Street = model.Street.Trim(),
            BuildingNumber = NullIfBlank(model.BuildingNumber),
            ApartmentNumber = NullIfBlank(model.ApartmentNumber),
            PostalCode = NullIfBlank(model.PostalCode),
            IsDefault = !existing.Any()
        });

        TempData["Success"] = "Address saved.";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /Addresses/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _addressService.DeleteUserAddressAsync(User.GetUserId(), id);

        if (deleted)
            TempData["Success"] = "Address removed.";
        else
            TempData["Error"] = "Address not found.";

        return RedirectToAction(nameof(Index));
    }

    private static string? NullIfBlank(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
