using EasyShop.Application.DTOs.Catalog;

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

        public async Task<Dictionary<int, ProductCardDto>> GetProductCardsAsync()
        {
            var images = await _unitOfWork.Products.GetPrimaryImageUrlsAsync();
            var variants = await _unitOfWork.ProductVariants.FindByConditionAsync(v => v.IsActive);

            var cards = variants
                .GroupBy(v => v.ProductId)
                .ToDictionary(g => g.Key, g => new ProductCardDto
                {
                    ProductId = g.Key,
                    FromPrice = g.Min(v => v.Price),
                    InStock = g.Any(v => v.StockQuantity > 0),
                    ImageUrl = images.GetValueOrDefault(g.Key)
                });

            // A product can have a picture before it has any variant on sale.
            foreach (var (productId, imageUrl) in images)
            {
                if (!cards.ContainsKey(productId))
                    cards[productId] = new ProductCardDto { ProductId = productId, ImageUrl = imageUrl };
            }

            return cards;
        }

        public async Task<IReadOnlyList<string>> GetImageUrlsAsync(int productId)
        {
            return await _unitOfWork.Products.GetImageUrlsAsync(productId);
        }

        public async Task<string?> GetPrimaryImageUrlAsync(int productId)
        {
            return await _unitOfWork.Products.GetPrimaryImageUrlAsync(productId);
        }

        public async Task SetPrimaryImageAsync(int productId, string? imageUrl)
        {
            await _unitOfWork.Products.SetPrimaryImageAsync(productId, imageUrl?.Trim());
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IReadOnlyList<ProductVariant>> GetVariantsAsync(int productId)
        {
            var variants = await _unitOfWork.ProductVariants.FindByConditionAsync(v => v.ProductId == productId);
            return variants.OrderBy(v => v.Price).ToList();
        }

        public async Task<ProductVariant?> GetVariantByIdAsync(int id)
        {
            return await _unitOfWork.ProductVariants.GetByIdAsync(id);
        }

        public async Task AddVariantAsync(ProductVariant variant)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(variant.ProductId)
                ?? throw new ArgumentException("The product was not found.");

            await EnsureVariantIsValidAsync(variant);

            variant.SKU = variant.SKU.Trim();
            variant.IsActive = variant.IsActive && product.IsActive;
            variant.CreatedAt = DateTime.UtcNow;
            variant.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ProductVariants.AddAsync(variant);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateVariantAsync(ProductVariant variant)
        {
            await EnsureVariantIsValidAsync(variant);

            variant.SKU = variant.SKU.Trim();
            variant.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ProductVariants.Update(variant);
            await _unitOfWork.CompleteAsync();
        }

        private async Task EnsureVariantIsValidAsync(ProductVariant variant)
        {
            if (string.IsNullOrWhiteSpace(variant.SKU))
                throw new ArgumentException("The SKU is required.");

            if (variant.Price <= 0)
                throw new ArgumentException("The price must be greater than zero.");

            if (variant.CompareAtPrice < 0 || (variant.CompareAtPrice > 0 && variant.CompareAtPrice < variant.Price))
                throw new ArgumentException("The original price must be empty or higher than the price.");

            if (variant.StockQuantity < 0)
                throw new ArgumentException("The stock cannot be negative.");

            var sku = variant.SKU.Trim();
            var duplicates = await _unitOfWork.ProductVariants
                .FindByConditionAsync(v => v.SKU == sku && v.Id != variant.Id);

            if (duplicates.Any())
                throw new ArgumentException($"The SKU \"{sku}\" is already used by another option.");
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
