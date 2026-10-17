namespace EasyShop.MVC.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;

    public ProductsController(
        IProductService productService,
        ICategoryService categoryService,
        IBrandService brandService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _brandService = brandService;
    }

    // GET: /Products
    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();

        ViewBag.BrandNames = (await _brandService.GetAllBrandsAsync()).ToDictionary(b => b.Id, b => b.Name);
        ViewBag.CategoryNames = (await _categoryService.GetAllCategoriesAsync()).ToDictionary(c => c.Id, c => c.Name);

        return View(products);
    }

    // GET: /Products/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        await LoadLookupNamesAsync(product);
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
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
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

        TempData["Success"] = $"Product \"{product.Name}\" was created.";
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
            IsActive = product.IsActive
        });
    }

    // POST: /Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

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

        await _productService.UpdateProductAsync(product);
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
        TempData["Success"] = $"Product \"{product.Name}\" was deleted.";
        return RedirectToAction(nameof(Index));
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
