
using EasyShop.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyShop.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task AddProductAsync(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(Product product);
    }
}
