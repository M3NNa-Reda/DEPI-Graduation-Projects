namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategory(int categoryId);
        Task<Dictionary<int, string>> GetPrimaryImageUrlsAsync();
        Task<IReadOnlyList<string>> GetImageUrlsAsync(int productId);
        Task<string?> GetPrimaryImageUrlAsync(int productId);
        Task SetPrimaryImageAsync(int productId, string? imageUrl);
    }
}
