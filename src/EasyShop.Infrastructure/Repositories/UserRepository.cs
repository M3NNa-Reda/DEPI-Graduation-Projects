namespace EasyShop.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _context.ApplicationUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser?> GetByEmailWithRolesAsync(string email)
        {
            return await _context.ApplicationUsers
                .AsNoTracking()
                .Include(u => u.ApplicationUserRoles)
                    .ThenInclude(ur => ur.ApplicationRole)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.ApplicationUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<ApplicationUser?> GetUserWithAddressesAsync(int userId)
        {
            return await _context.ApplicationUsers
                .AsNoTracking()
                .Include(u => u.UserAddresses)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser?> GetUserWithOrdersAsync(int userId)
        {
            return await _context.ApplicationUsers
                .AsNoTracking()
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<ApplicationUser>> GetActiveUsersAsync()
        {
            return await _context.ApplicationUsers
                .AsNoTracking()
                .Where(u => u.IsActive)
                .ToListAsync();
        }
    }
}