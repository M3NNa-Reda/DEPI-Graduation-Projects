using EasyShop.MVC.Services;

namespace EasyShop.MVC.Controllers;

[Authorize(Roles = RoleNames.Admin)]
public class ProductsController : Controller
{
    private const long MaxRequestBytes = 4 * 1024 * 1024;

    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;
    private readonly IProductImageStorage _imageStorage;

    public ProductsController(
        IProductService productService,
        ICategoryService categoryService,
        IBrandService brandService,
        IProductImageStorage imageStorage)
    {
        _productService = productService;
        _categoryService = categoryService;
        _brandService = brandService;
        _imageStorage = imageStorage;
    }

    // GET: /Products
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();

        // Customers only see products that are on sale; administrators manage all of them.
        if (!User.IsInRole(RoleNames.Admin))
        {
            products = products.Where(p => p.IsActive).ToList();
        }

        ViewBag.BrandNames = (await _brandService.GetAllBrandsAsync()).ToDictionary(b => b.Id, b => b.Name);
        ViewBag.CategoryNames = (await _categoryService.GetAllCategoriesAsync()).ToDictionary(c => c.Id, c => c.Name);

        ViewBag.Cards = await _productService.GetProductCardsAsync();

        return View(products);
    }

    // GET: /Products/Details/5
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        // Inactive products are hidden from everyone except administrators.
        if (!product.IsActive && !User.IsInRole(RoleNames.Admin))
        {
            return NotFound();
        }

        await LoadLookupNamesAsync(product);
        ViewBag.Variants = await _productService.GetActiveVariantsAsync(id);
        ViewBag.Images = await _productService.GetImageUrlsAsync(id);
        return View(product);
    }

    // GET: /Products/Create
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new ProductFormViewModel());
    }

    // POST: /Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxRequestBytes)]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        var imageUrl = ModelState.IsValid ? await ResolveImageAsync(model) : null;

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model.BrandId, model.CategoryId);
            return View(model);
        }

        var product = new Product
        {
            Name = model.Name,
            Description = model.Description ?? string.Empty,
            Slug = model.Slug,
            BrandId = model.BrandId,
            CategoryId = model.CategoryId,
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _productService.AddProductAsync(product);
        await _productService.SetPrimaryImageAsync(product.Id, imageUrl);

        TempData["Success"] = $"Product \"{product.Name}\" was created. Add its options (price and stock) so customers can buy it.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Products/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        await PopulateDropdownsAsync(product.BrandId, product.CategoryId);

        return View(new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Slug = product.Slug,
            BrandId = product.BrandId,
            CategoryId = product.CategoryId,
            IsActive = product.IsActive,
            ImageUrl = await _productService.GetPrimaryImageUrlAsync(id)
        });
    }

    // POST: /Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxRequestBytes)]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        var imageUrl = ModelState.IsValid ? await ResolveImageAsync(model) : null;

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model.BrandId, model.CategoryId);
            return View(model);
        }

        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.Name = model.Name;
        product.Description = model.Description ?? string.Empty;
        product.Slug = model.Slug;
        product.BrandId = model.BrandId;
        product.CategoryId = model.CategoryId;
        product.IsActive = model.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        var previousImageUrl = await _productService.GetPrimaryImageUrlAsync(id);

        await _productService.UpdateProductAsync(product);
        await _productService.SetPrimaryImageAsync(product.Id, imageUrl);

        // The old file is only deleted once the new picture is saved, so a failure never leaves the product without one.
        if (previousImageUrl != imageUrl)
        {
            _imageStorage.DeleteIfStored(previousImageUrl);
        }

        TempData["Success"] = $"Product \"{product.Name}\" was updated.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Products/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        await LoadLookupNamesAsync(product);
        return View(product);
    }

    // POST: /Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        await _productService.DeleteProductAsync(product);

        TempData["Success"] = $"Product \"{product.Name}\" was deactivated and is now hidden from customers.";
        return RedirectToAction(nameof(Index));
    }

    // Decides which picture the product ends up with: an uploaded file wins over a typed link,
    // and an empty link removes the picture. Problems are reported on the form, not thrown.
    private async Task<string?> ResolveImageAsync(ProductFormViewModel model)
    {
        if (model.ImageFile is { Length: > 0 })
        {
            try
            {
                return await _imageStorage.SaveAsync(model.ImageFile);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(model.ImageFile), ex.Message);
                return null;
            }
        }

        if (string.IsNullOrWhiteSpace(model.ImageUrl))
        {
            return null;
        }

        if (!_imageStorage.IsAcceptedUrl(model.ImageUrl.Trim()))
        {
            ModelState.AddModelError(nameof(model.ImageUrl), "Enter a link that starts with http:// or https://, or upload a picture.");
            return null;
        }

        return model.ImageUrl.Trim();
    }

    private async Task PopulateDropdownsAsync(int? brandId = null, int? categoryId = null)
    {
        ViewBag.Brands = new SelectList(await _brandService.GetAllBrandsAsync(), "Id", "Name", brandId);
        ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", categoryId);
    }

    private async Task LoadLookupNamesAsync(Product product)
    {
        ViewBag.BrandName = (await _brandService.GetBrandByIdAsync(product.BrandId))?.Name;
        ViewBag.CategoryName = product.CategoryId == null
            ? null
            : (await _categoryService.GetCategoryByIdAsync(product.CategoryId.Value))?.Name;
    }
}
