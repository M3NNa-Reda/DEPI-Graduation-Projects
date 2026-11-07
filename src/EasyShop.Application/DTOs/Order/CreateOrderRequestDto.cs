using System.ComponentModel.DataAnnotations;

namespace EasyShop.Application.DTOs.Order
{
    public class CreateOrderRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Choose a delivery address.")]
        public int AddressId { get; set; }

        [EnumDataType(typeof(PaymentMethod), ErrorMessage = "Choose a valid payment method.")]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(50, ErrorMessage = "The coupon code is too long.")]
        public string? CouponCode { get; set; }
    }

}
