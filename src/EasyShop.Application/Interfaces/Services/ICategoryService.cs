
using EasyShop.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyShop.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task AddCategoryAsync(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
    }
}
