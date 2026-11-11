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

        public async Task<string?> GetPrimaryImageUrlAsync(int productId)
        {
            return await _context.Set<ProductImage>()
                .AsNoTracking()
                .Where(i => i.ProductId == productId)
                .OrderByDescending(i => i.IsPrimary)
                .ThenBy(i => i.DisplayOrder)
                .Select(i => i.ImageUrl)
                .FirstOrDefaultAsync();
        }

        // Keeps exactly one primary image per product (the table has a unique filtered index on it),
        // so an existing row is updated in place rather than replaced. Saving is left to the unit of work.
        public async Task SetPrimaryImageAsync(int productId, string? imageUrl)
        {
            var images = _context.Set<ProductImage>();
            var primary = await images
                .Where(i => i.ProductId == productId)
                .OrderByDescending(i => i.IsPrimary)
                .ThenBy(i => i.DisplayOrder)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                if (primary != null)
                    images.Remove(primary);
                return;
            }

            if (primary == null)
            {
                images.Add(new ProductImage { ProductId = productId, ImageUrl = imageUrl, IsPrimary = true });
                return;
            }

            primary.ImageUrl = imageUrl;
            primary.IsPrimary = true;
        }
    }
}