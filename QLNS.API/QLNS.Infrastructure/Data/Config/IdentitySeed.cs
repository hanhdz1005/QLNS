using Microsoft.AspNetCore.Identity;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Infrastructure.Data.Config
{
    public class IdentitySeed
    {
        public static async Task SeedAdminAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));


            var admin = await userManager.FindByEmailAsync("admin@gmail.com");
            if (admin == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    DisplayName = "Administrator",
                    Role = "Admin",
                    Code = "ADMIN",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newAdmin, "Admin@1234");
                await userManager.AddToRoleAsync(newAdmin, "Admin");

                var claims = await userManager.GetClaimsAsync(newAdmin);

                if (!claims.Any(c => c.Type == "permission" && c.Value == "FULL.ACCESS"))
                {
                    await userManager.AddClaimAsync(newAdmin,
                        new Claim("permission", "FULL.ACCESS"));
                }

                if (!claims.Any(c => c.Type == "permission" && c.Value == "ASSET.VIEW"))
                {
                    await userManager.AddClaimAsync(newAdmin,
                        new Claim("permission", "ASSET.VIEW"));
                }
            }
        }
    }
}
