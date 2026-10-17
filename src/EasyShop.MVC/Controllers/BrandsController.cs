namespace EasyShop.MVC.Controllers;

public class BrandsController : Controller
{
    private readonly IBrandService _brandService;
    private readonly IProductService _productService;

    public BrandsController(IBrandService brandService, IProductService productService)
    {
        _brandService = brandService;
        _productService = productService;
    }

    // GET: /Brands
    public async Task<IActionResult> Index()
    {
        var brands = await _brandService.GetAllBrandsAsync();
        return View(brands);
    }

    // GET: /Brands/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var brand = await _brandService.GetBrandByIdAsync(id);
        if (brand == null)
        {
            return NotFound();
        }

        return View(brand);
    }

    // GET: /Brands/Create
    public IActionResult Create()
    {
        return View(new BrandFormViewModel());
    }

    // POST: /Brands/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BrandFormViewModel model)
    {
        if (await BrandNameExistsAsync(model.Name))
        {
            ModelState.AddModelError(nameof(model.Name), "A brand with this name already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var brand = new Brand
        {
            Name = model.Name.Trim(),
            Description = model.Description,
            LogoUrl = model.LogoUrl,
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _brandService.AddBrandAsync(brand);

        TempData["Success"] = $"Brand \"{brand.Name}\" was created.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Brands/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var brand = await _brandService.GetBrandByIdAsync(id);
        if (brand == null)
        {
            return NotFound();
        }

        return View(new BrandFormViewModel
        {
            Id = brand.Id,
            Name = brand.Name,
            Description = brand.Description,
            LogoUrl = brand.LogoUrl,
            IsActive = brand.IsActive
        });
    }

    // POST: /Brands/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BrandFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (await BrandNameExistsAsync(model.Name, id))
        {
            ModelState.AddModelError(nameof(model.Name), "A brand with this name already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var brand = await _brandService.GetBrandByIdAsync(id);
        if (brand == null)
        {
            return NotFound();
        }

        brand.Name = model.Name.Trim();
        brand.Description = model.Description;
        brand.LogoUrl = model.LogoUrl;
        brand.IsActive = model.IsActive;
        brand.UpdatedAt = DateTime.UtcNow;

        await _brandService.UpdateBrandAsync(brand);

        TempData["Success"] = $"Brand \"{brand.Name}\" was updated.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Brands/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var brand = await _brandService.GetBrandByIdAsync(id);
        if (brand == null)
        {
            return NotFound();
        }

        return View(brand);
    }

    // POST: /Brands/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var brand = await _brandService.GetBrandByIdAsync(id);
        if (brand == null)
        {
            return NotFound();
        }

        var products = await _productService.GetAllProductsAsync();
        if (products.Any(p => p.BrandId == id))
        {
            TempData["Error"] = $"Brand \"{brand.Name}\" can't be deleted while products still use it.";
            return RedirectToAction(nameof(Index));
        }

        await _brandService.DeleteBrandAsync(brand);

        TempData["Success"] = $"Brand \"{brand.Name}\" was deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> BrandNameExistsAsync(string name, int? excludeId = null)
    {
        var trimmed = name?.Trim();
        var brands = await _brandService.GetAllBrandsAsync();

        return brands.Any(b => b.Id != excludeId &&
              string.Equals(b.Name, trimmed, StringComparison.OrdinalIgnoreCase));
    }
}
