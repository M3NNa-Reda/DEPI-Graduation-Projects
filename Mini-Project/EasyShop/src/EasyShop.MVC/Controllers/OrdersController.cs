
using EasyShop.Application.Services;

namespace EasyShop.MVC.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IAddressService _addressService;
        private readonly ICartService _cartService;

        public OrdersController(IOrderService orderService, IAddressService addressService, ICartService cartService)
        {
            _orderService = orderService;
            _addressService = addressService;
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int userId = User.GetUserId();

            var cart = await _cartService.GetCartAsync(userId);
            if (!cart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty. Add something before checking out.";
                return RedirectToAction("Index", "Cart");
            }

            await PopulateCheckoutAsync(userId);
            return View(new CreateOrderRequestDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderRequestDto dto)
        {
            int userId = User.GetUserId();

            if (!ModelState.IsValid)
            {
                await PopulateCheckoutAsync(userId);
                return View(dto);
            }

            try
            {
                var result = await _orderService.CreateOrderAsync(userId, dto);
                TempData["Success"] = "Your order has been placed.";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
            {
                // Business rule failures (invalid coupon, empty cart, not enough stock...)
                // are shown on the form instead of crashing the request.
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateCheckoutAsync(userId);
                return View(dto);
            }
        }

        private async Task PopulateCheckoutAsync(int userId)
        {
            var addresses = await _addressService.GetAddressesByUserIdAsync(userId);

            ViewBag.Addresses = new SelectList(addresses, "Id", "FullAddress");
            ViewBag.Cart = await _cartService.GetCartAsync(userId);
        }
        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            int userId = User.GetUserId();
            var orders = await _orderService.GetMyOrdersAsync(userId);

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            int userId = User.GetUserId();
            var order = await _orderService.GetMyOrderByIdAsync(userId, id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            int userId = User.GetUserId();

            try
            {
                await _orderService.CancelOrderAsync(userId, id);
                TempData["Success"] = "The order has been cancelled.";
            }
            catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            return RedirectToAction(nameof(MyOrders));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RecentOrders(int page = 1, int pageSize = 10)
        {
            var orders = await _orderService.GetRecentOrdersAsync(page, pageSize);
            return View(orders);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDetails(int id)
        {
            var order = await _orderService.GetOrderByIdForAdminAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = ModelState.Values
                    .SelectMany(entry => entry.Errors)
                    .Select(error => error.ErrorMessage)
                    .FirstOrDefault(message => !string.IsNullOrEmpty(message)) ?? "Invalid status.";
                return RedirectToAction(nameof(AdminDetails), new { id });
            }

            int adminId = User.GetUserId();

            try
            {
                await _orderService.UpdateOrderStatusAsync(adminId, id, dto);
                TempData["Success"] = "The order status has been updated.";
            }
            catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException or ArgumentException)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(AdminDetails), new { id });
        }
    }
}