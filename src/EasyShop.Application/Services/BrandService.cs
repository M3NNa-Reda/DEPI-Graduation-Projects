namespace EasyShop.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Brand?> GetBrandByIdAsync(int id)
        {
            return await _unitOfWork.Brands.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
        {
            return await _unitOfWork.Brands.GetAllAsync();
        }

        public async Task AddBrandAsync(Brand brand)
        {
            await _unitOfWork.Brands.AddAsync(brand);
            await _unitOfWork.CompleteAsync();
        }

        public void UpdateBrand(Brand brand)
        {
            _unitOfWork.Brands.Update(brand);
            _unitOfWork.CompleteAsync().GetAwaiter().GetResult();
        }

        public void DeleteBrand(Brand brand)
        {
            _unitOfWork.Brands.Delete(brand);
            _unitOfWork.CompleteAsync().GetAwaiter().GetResult();
        }
    }
}
