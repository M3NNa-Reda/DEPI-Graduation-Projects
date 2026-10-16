namespace EasyShop.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int BrandId { get; set; } //fk
        public int? CategoryId { get; set; } //fk
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Brand? Brand { get; set; }
        public Category? Category { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();
        public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();

    }
}
