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
    }
}
