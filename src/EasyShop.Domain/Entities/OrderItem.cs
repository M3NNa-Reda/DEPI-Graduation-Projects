namespace EasyShop.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderId { get; set; } //FK
        public Order Order { get; set; }
        public int ProductVariantId { get; set; } //FK
        public ProductVariant ProductVariant { get; set; }
        public Review? Review { get; set; }
    }
}
