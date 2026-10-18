using Microsoft.AspNetCore.Identity;
using QLNSCore.Entities;

namespace QLNSCore.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Role { get; set; }
        public string Code { get; set; }
        public string? AvatarUrl { get; set; }
        public string? RefreshToken { get; set; }
        public System.DateTime? RefreshTokenExpiry { get; set; }
    }
}
