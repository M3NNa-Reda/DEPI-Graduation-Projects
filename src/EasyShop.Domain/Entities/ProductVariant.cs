using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Domain.Entities
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public decimal CompareAtPrice { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; }
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Product Product { get; set; }
        public ICollection<ProductVariantOptionValue> productVariantOptionValues { get; set; } = new List<ProductVariantOptionValue>();
        public ICollection<InventoryTransaction> inventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
