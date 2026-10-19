using EasyShop.Application.DTOs.Auth;
using EasyShop.Domain.Constants;

namespace EasyShop.Application.Services
{
    public class AuthService : IAuthService
    {
        private const string InvalidCredentialsMessage = "Invalid email or password.";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterRequestDto request)
        {
            var email = NormalizeEmail(request.Email);

            if (await _unitOfWork.Users.GetByEmailAsync(email) != null)
            {
                return AuthResultDto.Failure("An account with this email already exists.");
            }

            // The role is always assigned here, never taken from the request.
            var customerRole = (await _unitOfWork.Roles
                    .FindByConditionAsync(r => r.Name == RoleNames.Customer && r.IsActive))
                .FirstOrDefault()
                ?? throw new InvalidOperationException("The Customer role is not configured.");

            var user = new ApplicationUser
            {
                Email = email,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
                PasswordHash = _passwordHasher.Hash(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ApplicationUserRoles = { new ApplicationUserRole { RoleId = customerRole.Id } }
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return AuthResultDto.Success(ToDto(user, new[] { customerRole.Name }));
        }

        public async Task<AuthResultDto> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
            {
                return AuthResultDto.Failure(InvalidCredentialsMessage);
            }

            var user = await _unitOfWork.Users.GetByEmailWithRolesAsync(NormalizeEmail(email));

            // Verify even when the email is unknown so the response time does not reveal which emails exist.
            var passwordIsValid = _passwordHasher.Verify(password, user?.PasswordHash);

            if (user == null || !passwordIsValid)
            {
                return AuthResultDto.Failure(InvalidCredentialsMessage);
            }

            if (!user.IsActive)
            {
                return AuthResultDto.Failure("This account has been deactivated.");
            }

            var roles = user.ApplicationUserRoles
                .Where(ur => ur.ApplicationRole.IsActive)
                .Select(ur => ur.ApplicationRole.Name)
                .ToList();

            return AuthResultDto.Success(ToDto(user, roles));
        }

        private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

        private static AuthenticatedUserDto ToDto(ApplicationUser user, IReadOnlyList<string> roles) => new()
        {
            Id = user.Id,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            Roles = roles
        };
    }
}
