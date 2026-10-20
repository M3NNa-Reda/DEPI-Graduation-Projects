using EasyShop.Application.DTOs.Auth;

namespace EasyShop.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResultDto> LoginAsync(string email, string password);
    }
}
