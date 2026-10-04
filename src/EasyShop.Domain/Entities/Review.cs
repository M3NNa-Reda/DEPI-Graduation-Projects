using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; } //FK
        public ApplicationUser ApplicationUser { get; set; }
        public int OrderItemId { get; set; } //FK
        public OrderItem OrderItem { get; set; }
    }
}
