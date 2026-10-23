namespace EasyShop.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.Products.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _unitOfWork.Products.GetProductsByCategory(categoryId);
        }

        public async Task<IEnumerable<ProductVariant>> GetActiveVariantsAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null || !product.IsActive)
                return Enumerable.Empty<ProductVariant>();

            var variants = await _unitOfWork.ProductVariants
                .FindByConditionAsync(v => v.ProductId == productId && v.IsActive);

            return variants.OrderBy(v => v.Price).ToList();
        }

        public async Task AddProductAsync(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _unitOfWork.Products.Update(product);
            await SyncVariantsWithProductAsync(product);
            await _unitOfWork.CompleteAsync();
        }

        // Soft delete: the product and its variants are kept (orders reference them)
        // but become inactive, so they can no longer be listed, added to a cart or ordered.
        public async Task DeleteProductAsync(Product product)
        {
            product.IsActive = false;
            _unitOfWork.Products.Update(product);
            await SyncVariantsWithProductAsync(product);
            await _unitOfWork.CompleteAsync();
        }

        // Variants follow the product's state: deactivating a product deactivates all of its
        // variants, and reactivating it makes them available again.
        private async Task SyncVariantsWithProductAsync(Product product)
        {
            var variants = await _unitOfWork.ProductVariants
                .FindByConditionAsync(v => v.ProductId == product.Id && v.IsActive != product.IsActive);

            foreach (var variant in variants)
            {
                variant.IsActive = product.IsActive;
                variant.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
