
using EasyShop.Application.Services;

namespace EasyShop.MVC.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IAddressService _addressService;

        public OrdersController(IOrderService orderService, IAddressService addressService)
        {
            _orderService = orderService;
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int userId = User.GetUserId();
            await PopulateAddressesSelectList(userId);
            return View(new CreateOrderRequestDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderRequestDto dto)
        {
            int userId = User.GetUserId();

            if (!ModelState.IsValid)
            {
                await PopulateAddressesSelectList(userId);
                return View(dto);
            }

            var result = await _orderService.CreateOrderAsync(userId, dto);
            return RedirectToAction(nameof(Details), new { id = result.Id });
        }

        private async Task PopulateAddressesSelectList(int userId)
        {
            var addresses = await _addressService.GetAddressesByUserIdAsync(userId);

            ViewBag.Addresses = new SelectList(addresses, "Id", "FullAddress");
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
            await _orderService.CancelOrderAsync(userId, id);

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
            int adminId = User.GetUserId();
            await _orderService.UpdateOrderStatusAsync(adminId, id, dto);

            return RedirectToAction(nameof(AdminDetails), new { id });
        }
    }
}