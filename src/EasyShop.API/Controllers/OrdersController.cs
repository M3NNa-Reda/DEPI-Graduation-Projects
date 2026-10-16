using EasyShop.API.Helpers.Extensions;
using EasyShop.Application.DTOs.Order;
using EasyShop.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        public async Task<ActionResult<OrderDetailsDto>> CreateOrder([FromBody] CreateOrderRequestDto dto)
        {
            int userId = User.GetUserId();
            var result = await _orderService.CreateOrderAsync(userId, dto);
            return Ok(result);
        }
        [HttpGet("my-orders")]
        public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetMyOrders()
        {
            int userId = User.GetUserId();
            var orders = await _orderService.GetMyOrdersAsync(userId);
            return Ok(orders);
        }
        [HttpGet("my-orders/{id}")]
        public async Task<ActionResult<OrderDetailsDto>> GetMyOrderById(int id)
        {
            int userId = User.GetUserId();
            var orders = await _orderService.GetMyOrderByIdAsync(userId, id);
            return Ok(orders);

        }
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            int userId = User.GetUserId();
            var orders = _orderService.CancelOrderAsync(userId, id);
            return NoContent();
        }
        [HttpGet("admin/recent")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetRecentOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var orders = await _orderService.GetRecentOrdersAsync(page, pageSize);
            return Ok(orders);
        }
        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderByIdForAdmin(int id)
        {
            var order= await _orderService.GetOrderByIdForAdminAsync(id);
            return Ok(order);
        }
        [HttpPut("admin/{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            int adminId = User.GetUserId();
            var task = _orderService.UpdateOrderStatusAsync(adminId, id, dto);
            return NoContent();
        }
    }
}
