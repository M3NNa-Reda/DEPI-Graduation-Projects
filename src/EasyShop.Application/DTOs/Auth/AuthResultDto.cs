namespace EasyShop.Application.DTOs.Auth
{
    public class AuthResultDto
    {
        public bool Succeeded { get; private init; }
        public string? Error { get; private init; }
        public AuthenticatedUserDto? User { get; private init; }

        public static AuthResultDto Success(AuthenticatedUserDto user) =>
            new() { Succeeded = true, User = user };

        public static AuthResultDto Failure(string error) =>
            new() { Error = error };
    }
}
