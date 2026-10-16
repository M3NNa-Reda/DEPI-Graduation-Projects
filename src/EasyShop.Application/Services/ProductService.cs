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

        public async Task AddProductAsync(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
        }

        public void UpdateProduct(Product product)
        {
            _unitOfWork.Products.Update(product);
            _unitOfWork.CompleteAsync().GetAwaiter().GetResult();
        }

        public void DeleteProduct(Product product)
        {
            _unitOfWork.Products.Delete(product);
            _unitOfWork.CompleteAsync().GetAwaiter().GetResult();
        }
    }
}
