namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetByEmailAsync(string email);

        Task<ApplicationUser?> GetByPhoneNumberAsync(string phoneNumber);

        Task<ApplicationUser?> GetUserWithAddressesAsync(int userId);

        Task<ApplicationUser?> GetUserWithOrdersAsync(int userId);

        Task<IEnumerable<ApplicationUser>> GetActiveUsersAsync();
    }
}