namespace EasyShop.Application.Interfaces.Services
{
    public interface IBrandService
    {
        Task<Brand?> GetBrandByIdAsync(int id);
        Task<IEnumerable<Brand>> GetAllBrandsAsync();
        Task AddBrandAsync(Brand brand);
        void UpdateBrand(Brand brand);
        void DeleteBrand(Brand brand);
    }
}
