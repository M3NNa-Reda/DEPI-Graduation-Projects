namespace EasyShop.Infrastructure.Repositories
{
    public class ProductVariantRepository : GenericRepository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ProductVariant>> GetVariantsByIdsAsync(IEnumerable<int> variantIds)
        {
            return await _context.ProductVariants
                .Where(pv => variantIds.Contains(pv.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductVariant>> GetVariantsWithProductByIdsAsync(IEnumerable<int> variantIds)
        {
            return await _context.ProductVariants
                .Where(pv => variantIds.Contains(pv.Id))
                .Include(pv => pv.Product)
                .ToListAsync();
        }
    }
}
