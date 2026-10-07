using EasyShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Domain.Entities
{
    public class InventoryTransaction
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public TransactionType TransactionType { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProductVariantId { get; set; } //FK
        public ProductVariant ProductVariant { get; set; }
        public ProductVariant  productVariant { get; set; }
    }
}
