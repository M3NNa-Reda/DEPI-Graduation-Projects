namespace EasyShop.MVC.Helpers.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this System.Security.Claims.ClaimsPrincipal user)
        {
            var claim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (claim == null || !int.TryParse(claim, out int userId))
            {
                throw new UnauthorizedAccessException("User ID is missing or invalid in token.");
            }
            return userId;
        }
    }
}
