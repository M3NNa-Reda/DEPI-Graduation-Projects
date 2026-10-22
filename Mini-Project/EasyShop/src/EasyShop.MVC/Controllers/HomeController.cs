namespace EasyShop.MVC;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly IBrandService _brandService;
    private readonly ICategoryService _categoryService;
    private readonly IOrderService _orderService;
    private readonly ICouponService _couponService;
    private readonly IPaymentService _paymentService;
    private readonly ICartService _cartService;
    private readonly IAddressService _addressService;

    public HomeController(
        IProductService productService,
        IBrandService brandService,
        ICategoryService categoryService,
        IOrderService orderService,
        ICouponService couponService,
        IPaymentService paymentService,
        ICartService cartService,
        IAddressService addressService)
    {
        _productService = productService;
        _brandService = brandService;
        _categoryService = categoryService;
        _orderService = orderService;
        _couponService = couponService;
        _paymentService = paymentService;
        _cartService = cartService;
        _addressService = addressService;
    }

    public async Task<IActionResult> Index()
    {
        if (User.IsInRole(RoleNames.Admin))
        {
            return View(await BuildAdminDashboardAsync());
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            return View(await BuildCustomerDashboardAsync(User.GetUserId()));
        }

        return View(await BuildVisitorDashboardAsync());
    }

    private async Task<HomeViewModel> BuildAdminDashboardAsync()
    {
        var products = await _productService.GetAllProductsAsync();
        var brands = await _brandService.GetAllBrandsAsync();
        var categories = await _categoryService.GetAllCategoriesAsync();
        var coupons = await _couponService.GetAllCouponsAsync();
        var orders = await _orderService.GetRecentOrdersAsync(1, 100);
        var payments = await _paymentService.GetAllPaymentsAsync();

        return new HomeViewModel
        {
            Title = "EasyShop Dashboard",
            Subtitle = "A quick overview of your store.",
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
                    Title = "Coupons",
                    Count = coupons.Count().ToString(),
                    Description = "Discount codes, their limits and validity dates.",
                    Controller = "Coupons"
                },
                new()
                {
                    Title = "Orders",
                    Count = orders.Count().ToString(),
                    Description = "Manage customer orders, view details, and update status.",
                    Controller = "Orders",
                    Action = "RecentOrders"
                },
                new()
                {
                    Title = "Payments",
                    Count = payments.Count().ToString(),
                    Description = "Review payments and their transaction history.",
                    Controller = "Payments"
                }
            }
        };
    }

    private async Task<HomeViewModel> BuildCustomerDashboardAsync(int userId)
    {
        var products = await _productService.GetAllProductsAsync();
        var cart = await _cartService.GetCartAsync(userId);
        var orders = await _orderService.GetMyOrdersAsync(userId);
        var addresses = await _addressService.GetAddressesByUserIdAsync(userId);

        return new HomeViewModel
        {
            Title = "Welcome back",
            Subtitle = "Pick up where you left off.",
            Cards = new List<StatCardViewModel>
            {
                new()
                {
                    Title = "Shop",
                    Count = products.Count().ToString(),
                    Description = "Browse the catalog and add items to your cart.",
                    Controller = "Products",
                    ButtonText = "Browse products"
                },
                new()
                {
                    Title = "My cart",
                    Count = cart.Items.Count.ToString(),
                    Description = "Review what you picked and check out.",
                    Controller = "Cart",
                    ButtonText = "View cart"
                },
                new()
                {
                    Title = "My orders",
                    Count = orders.Count().ToString(),
                    Description = "Track your orders and their status.",
                    Controller = "Orders",
                    Action = "MyOrders",
                    ButtonText = "View orders"
                },
                new()
                {
                    Title = "My addresses",
                    Count = addresses.Count().ToString(),
                    Description = "Delivery addresses you can choose at checkout.",
                    Controller = "Addresses"
                }
            }
        };
    }

    private async Task<HomeViewModel> BuildVisitorDashboardAsync()
    {
        var products = await _productService.GetAllProductsAsync();

        return new HomeViewModel
        {
            Title = "Welcome to EasyShop",
            Subtitle = "Browse the catalog, then log in or register to start shopping.",
            Cards = new List<StatCardViewModel>
            {
                new()
                {
                    Title = "Products",
                    Count = products.Count().ToString(),
                    Description = "See what we sell, with pricing and availability.",
                    Controller = "Products",
                    ButtonText = "Browse products"
                }
            }
        };
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
