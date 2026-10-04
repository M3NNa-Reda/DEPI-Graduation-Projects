using EasyShop.Domain.Enums;
namespace EasyShop.Domain.Entities
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? MaximumDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<CouponUsage> Usages { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
