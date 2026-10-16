using EasyShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.DTOs.Order
{
    // order card --> client -> my order
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalItems { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
