namespace EasyShop.MVC.Controllers;

// Prices and stock live on the options (variants) of a product, so a product only becomes buyable once it has one.
[Authorize(Roles = RoleNames.Admin)]
public class VariantsController : Controller
{
    private readonly IProductService _productService;

    public VariantsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: /Variants?productId=5
    public async Task<IActionResult> Index(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
        {
            return NotFound();
        }

        ViewBag.Product = product;
        return View(await _productService.GetVariantsAsync(productId));
    }

    // GET: /Variants/Create?productId=5
    public async Task<IActionResult> Create(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
        {
            return NotFound();
        }

        ViewBag.Product = product;
        return View(new VariantFormViewModel { ProductId = productId });
    }

    // POST: /Variants/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VariantFormViewModel model)
    {
        var product = await _productService.GetProductByIdAsync(model.ProductId);
        if (product == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _productService.AddVariantAsync(new ProductVariant
                {
                    ProductId = model.ProductId,
                    SKU = model.SKU,
                    Price = model.Price,
                    CompareAtPrice = model.CompareAtPrice,
                    StockQuantity = model.StockQuantity,
                    IsActive = model.IsActive
                });

                TempData["Success"] = $"Option \"{model.SKU.Trim()}\" was added.";
                return RedirectToAction(nameof(Index), new { productId = model.ProductId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        ViewBag.Product = product;
        return View(model);
    }

    // GET: /Variants/Edit/7
    public async Task<IActionResult> Edit(int id)
    {
        var variant = await _productService.GetVariantByIdAsync(id);
        if (variant == null)
        {
            return NotFound();
        }

        ViewBag.Product = await _productService.GetProductByIdAsync(variant.ProductId);
        return View(new VariantFormViewModel
        {
            Id = variant.Id,
            ProductId = variant.ProductId,
            SKU = variant.SKU,
            Price = variant.Price,
            CompareAtPrice = variant.CompareAtPrice,
            StockQuantity = variant.StockQuantity,
            IsActive = variant.IsActive
        });
    }

    // POST: /Variants/Edit/7
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VariantFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        var variant = await _productService.GetVariantByIdAsync(id);
        if (variant == null)
        {
            return NotFound();
        }

        var product = await _productService.GetProductByIdAsync(variant.ProductId);

        if (ModelState.IsValid)
        {
            variant.SKU = model.SKU;
            variant.Price = model.Price;
            variant.CompareAtPrice = model.CompareAtPrice;
            variant.StockQuantity = model.StockQuantity;
            // An option of a deactivated product stays off until the product itself is reactivated.
            variant.IsActive = model.IsActive && product is { IsActive: true };

            try
            {
                await _productService.UpdateVariantAsync(variant);

                TempData["Success"] = $"Option \"{variant.SKU}\" was updated.";
                return RedirectToAction(nameof(Index), new { productId = variant.ProductId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        ViewBag.Product = product;
        model.ProductId = variant.ProductId;
        return View(model);
    }
}
