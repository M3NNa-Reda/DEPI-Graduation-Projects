using EasyShop.Application.DTOs.User;
using EasyShop.Domain.Entities;

namespace EasyShop.Application.Interfaces.Services
{
    public interface IAddressService
    {
        Task<UserAddress?> GetAddressByIdAsync(int id);
        Task<IEnumerable<UserAddress>> GetAllAddressesAsync();
        Task<IEnumerable<AddressDto>> GetAddressesByUserIdAsync(int userId);
        Task AddAddressAsync(UserAddress address);
        void UpdateAddress(UserAddress address);
        void DeleteAddress(UserAddress address);
    }
}