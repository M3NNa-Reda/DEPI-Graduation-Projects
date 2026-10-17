using EasyShop.Domain.Entities;

namespace EasyShop.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByIdAsync(int id);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<ApplicationUser?> GetUserWithAddressesAsync(int userId);
        Task<ApplicationUser?> GetUserWithOrdersAsync(int userId);
        Task<IEnumerable<ApplicationUser>> GetActiveUsersAsync();
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task AddUserAsync(ApplicationUser user);
        void UpdateUser(ApplicationUser user);
        void DeleteUser(ApplicationUser user);
    }
}