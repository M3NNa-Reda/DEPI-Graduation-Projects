namespace EasyShop.MVC;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly IBrandService _brandService;
    private readonly ICategoryService _categoryService;
    private readonly IOrderService _orderService;

    public HomeController(
        IProductService productService,
        IBrandService brandService,
        ICategoryService categoryService,
        IOrderService orderService)
    {
        _productService = productService;
        _brandService = brandService;
        _categoryService = categoryService;
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        var brands = await _brandService.GetAllBrandsAsync();
        var categories = await _categoryService.GetAllCategoriesAsync();
        var orders = await _orderService.GetRecentOrdersAsync(1, 100);

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
                },
                new()
                {
                    Title = "Orders",
                    Count = orders.Count().ToString(), 
                    Description = "Manage customer orders, view details, and update status.",
                    Controller = "Orders",
                    Action = "RecentOrders"
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