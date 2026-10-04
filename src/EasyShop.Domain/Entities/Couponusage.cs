namespace EasyShop.Domain.Entities
{
    public class CouponUsage
    {
        public int Id { get; set; }
        public int CouponId { get; set; }
        public string UserId { get; set; }
        public int OrderId { get; set; }
        public DateTime UsedAt { get; set; }
        public Coupon Coupon { get; set; }
        public ApplicationUser User { get; set; }
        public Order Order { get; set; }
    }
}
