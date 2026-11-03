using EasyShop.Application.DTOs.Catalog;

namespace EasyShop.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductVariant>> GetActiveVariantsAsync(int productId);
        Task<Dictionary<int, ProductCardDto>> GetProductCardsAsync();
        Task<IReadOnlyList<string>> GetImageUrlsAsync(int productId);
        Task<string?> GetPrimaryImageUrlAsync(int productId);
        Task SetPrimaryImageAsync(int productId, string? imageUrl);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
    }
}
