using EasyShop.Application.DTOs.User;
using EasyShop.Domain.Entities;

namespace EasyShop.Application.Interfaces.Services
{
    public interface IAddressService
    {
        Task<UserAddress?> GetAddressByIdAsync(int id);
        Task<IEnumerable<UserAddress>> GetAllAddressesAsync();
        Task<IEnumerable<AddressDto>> GetAddressesByUserIdAsync(int userId);
        Task<IEnumerable<UserAddress>> GetUserAddressesAsync(int userId);
        Task<bool> DeleteUserAddressAsync(int userId, int addressId);
        Task AddAddressAsync(UserAddress address);
        Task UpdateAddressAsync(UserAddress address);
        Task DeleteAddressAsync(UserAddress address);
    }
}