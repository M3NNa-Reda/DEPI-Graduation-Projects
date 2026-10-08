namespace EasyShop.Domain.Entities
{
    public class CouponUsage
    {
        public int Id { get; set; }
        public int CouponId { get; set; } // fk
        public int UserId { get; set; } // fk
        public int OrderId { get; set; } //fk
        public DateTime UsedAt { get; set; }
        public Coupon Coupon { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Order Order { get; set; }
    }
}
