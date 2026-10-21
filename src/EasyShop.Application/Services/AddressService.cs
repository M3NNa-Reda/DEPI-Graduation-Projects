using EasyShop.Application.DTOs.User;
using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Application.Interfaces.Services;
using EasyShop.Domain.Entities;

namespace EasyShop.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserAddress?> GetAddressByIdAsync(int id)
        {
            return await _unitOfWork.UserAddresses.GetByIdAsync(id);
        }

        public async Task<IEnumerable<UserAddress>> GetAllAddressesAsync()
        {
            return await _unitOfWork.UserAddresses.GetAllAsync();
        }

        public async Task<IEnumerable<AddressDto>> GetAddressesByUserIdAsync(int userId)
        {
            var addresses = await _unitOfWork.UserAddresses
                .FindByConditionAsync(a => a.UserId == userId);

            return addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                FullAddress = $"{a.City}, {a.Area}, {a.Street}" +
                              (!string.IsNullOrEmpty(a.BuildingNumber) ? $", Bldg: {a.BuildingNumber}" : "") +
                              (!string.IsNullOrEmpty(a.ApartmentNumber) ? $", Apt: {a.ApartmentNumber}" : "")
            }).ToList();
        }
        public async Task AddAddressAsync(UserAddress address)
        {
            await _unitOfWork.UserAddresses.AddAsync(address);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAddressAsync(UserAddress address)
        {
            _unitOfWork.UserAddresses.Update(address);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAddressAsync(UserAddress address)
        {
            _unitOfWork.UserAddresses.Delete(address);
            await _unitOfWork.CompleteAsync();
        }
    }
}