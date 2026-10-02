using EasyShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Domain.Entities
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }
        public OrderStatus? OldStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public string? Notes { get; set; }
        public DateTime ChangedAt { get; set; }
        public int OrderId { get; set; } //FK
        public Order Order { get; set; }
        public int? ChangedByUserId { get; set; } //FK
    }
}
