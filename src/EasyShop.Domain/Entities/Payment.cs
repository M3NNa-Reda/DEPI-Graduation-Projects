using EasyShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public Order Order { get; set; }
        public ICollection<PaymentTransaction> Transactions { get; set; }
    }
}

