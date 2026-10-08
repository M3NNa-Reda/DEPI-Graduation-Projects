using EasyShop.Domain.Enums;
namespace EasyShop.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; } //FK
        public ApplicationUser ApplicationUser { get; set; }
        public int? CouponId { get; set; } //FK
        public Coupon Coupon { get; set; }
        public Payment Payment { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
        public OrderShippingAddress OrderShippingAddress { get; set; }
        public CouponUsage? CouponUsage { get; set; }
        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();


    }
}
