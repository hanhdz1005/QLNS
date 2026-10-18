using System.Security.Claims;

namespace QLNS.API.Extension
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserIdentifier(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value
                ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.Identity?.Name;
        }
    }
}
