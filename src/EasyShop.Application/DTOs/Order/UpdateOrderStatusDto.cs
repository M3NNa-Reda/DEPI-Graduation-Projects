using System.ComponentModel.DataAnnotations;

namespace EasyShop.Application.DTOs.Order
{
    public class UpdateOrderStatusDto
    {
        [EnumDataType(typeof(OrderStatus), ErrorMessage = "Choose a valid status.")]
        public OrderStatus Status { get; set; }

        [StringLength(1000, ErrorMessage = "Notes can be at most 1000 characters.")]
        public string? Notes { get; set; }
    }
}
