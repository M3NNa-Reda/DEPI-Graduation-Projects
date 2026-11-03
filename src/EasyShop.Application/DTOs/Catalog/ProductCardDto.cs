namespace EasyShop.Application.DTOs.Catalog
{
    // Everything the product list needs to draw one card, so the view does not load whole entities.
    public class ProductCardDto
    {
        public int ProductId { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? FromPrice { get; set; }
        public bool InStock { get; set; }
    }
}
