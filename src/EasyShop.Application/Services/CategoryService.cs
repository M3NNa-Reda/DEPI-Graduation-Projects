
using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Application.Interfaces.Services;
using EasyShop.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyShop.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _unitOfWork.Categories.GetById(id);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.Categories.GetAll();
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();
        }

        public void UpdateCategory(Category category)
        {
            _unitOfWork.Categories.Update(category);
            _unitOfWork.CompleteAsync().GetAwaiter().GetResult();
        }

        public void DeleteCategory(Category category)
        {
            _unitOfWork.Categories.Delete(category);
            _unitOfWork.CompleteAsync().GetAwaiter().GetResult();
        }
    }
}
