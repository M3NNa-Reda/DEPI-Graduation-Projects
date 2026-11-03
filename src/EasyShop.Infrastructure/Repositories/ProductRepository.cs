namespace EasyShop.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(int categoryId)
        {
            return await _context.Set<Product>()
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
       }

        public async Task<Dictionary<int, string>> GetPrimaryImageUrlsAsync()
        {
            var images = await _context.Set<ProductImage>()
                .AsNoTracking()
                .OrderByDescending(i => i.IsPrimary)
                .ThenBy(i => i.DisplayOrder)
                .ToListAsync();

            return images
                .GroupBy(i => i.ProductId)
                .ToDictionary(g => g.Key, g => g.First().ImageUrl);
        }

        public async Task<IReadOnlyList<string>> GetImageUrlsAsync(int productId)
        {
            return await _context.Set<ProductImage>()
                .AsNoTracking()
                .Where(i => i.ProductId == productId)
                .OrderByDescending(i => i.IsPrimary)
                .ThenBy(i => i.DisplayOrder)
                .Select(i => i.ImageUrl)
                .ToListAsync();
        }
    }
}
