using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Domain.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<ApplicationUserRole> ApplicationUserRoles { get; set; } = new List<ApplicationUserRole>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
        public ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
        public Cart Cart { get; set; }
        public Wishlist Wishlist { get; set; }
        public ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
    }
}
