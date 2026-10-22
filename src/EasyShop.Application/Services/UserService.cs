using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Application.Interfaces.Services;
using EasyShop.Domain.Entities;

namespace EasyShop.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(int id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.Users.GetByEmailAsync(email);
        }

        public async Task<ApplicationUser?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await _unitOfWork.Users.GetByPhoneNumberAsync(phoneNumber);
        }

        public async Task<ApplicationUser?> GetUserWithAddressesAsync(int userId)
        {
            return await _unitOfWork.Users.GetUserWithAddressesAsync(userId);
        }

        public async Task<ApplicationUser?> GetUserWithOrdersAsync(int userId)
        {
            return await _unitOfWork.Users.GetUserWithOrdersAsync(userId);
        }

        public async Task<IEnumerable<ApplicationUser>> GetActiveUsersAsync()
        {
            return await _unitOfWork.Users.GetActiveUsersAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _unitOfWork.Users.GetAllAsync();
        }

        public async Task AddUserAsync(ApplicationUser user)
        {
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateUserAsync(ApplicationUser user)
        {
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteUserAsync(ApplicationUser user)
        {
            _unitOfWork.Users.Delete(user);
            await _unitOfWork.CompleteAsync();
        }
    }
}