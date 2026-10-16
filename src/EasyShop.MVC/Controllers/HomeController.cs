namespace EasyShop.MVC;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly IBrandService _brandService;
    private readonly ICategoryService _categoryService;

    public HomeController(
        IProductService productService,
        IBrandService brandService,
        ICategoryService categoryService)
    {
        _productService = productService;
        _brandService = brandService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        var brands = await _brandService.GetAllBrandsAsync();
        var categories = await _categoryService.GetAllCategoriesAsync();

        var model = new HomeViewModel
        {
            Cards = new List<StatCardViewModel>
            {
                new()
                {
                    Title = "Products",
                    Count = products.Count().ToString(),
                    Description = "Everything you sell, with pricing and stock.",
                    Controller = "Products"
                },
                new()
                {
                    Title = "Brands",
                    Count = brands.Count().ToString(),
                    Description = "The manufacturers and labels behind your products.",
                    Controller = "Brands"
                },
                new()
                {
                    Title = "Categories",
                    Count = categories.Count().ToString(),
                    Description = "Organise products into browsable groups.",
                    Controller = "Categories"
                }
            }
        };

        return View(model);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}