namespace EasyShop.Application.DTOs.Order
{
    public class CreateOrderRequestDto
    {
        public int AddressId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? CouponCode { get; set; }
    }

}
