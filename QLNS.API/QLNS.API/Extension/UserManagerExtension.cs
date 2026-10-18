using Microsoft.AspNetCore.Identity;
using QLNSCore.Entities;
using System.Security.Claims;

namespace QLNS.API.Extension
{
    public static class UserManagerExtension
    {
        public static async Task<AppUser> FindEmailByClaimPrincipal(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var email = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            return await userManager.FindByEmailAsync(email);
        }
    }
}
