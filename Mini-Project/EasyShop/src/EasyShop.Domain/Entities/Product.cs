using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int BrandId { get; set; } //fk
        public int? CategoryId { get; set; } //fk
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Brand Brand { get; set; }
        public Category Category { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();
        public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();

    }
}
