using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLNS.Core.Common;
using QLNSCore.Entities;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QLNS.Infrastructure.Data.Config
{
    public class IdentitySeed
    {
        public static async Task SeedAdminAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Đảm bảo 3 role chính luôn tồn tại: Admin, Accountant, Employee
            foreach (var roleName in RoleConstants.CoreRoles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Đồng bộ các tài khoản cũ từ User / Leader / Kế toán sang 3 role chuẩn
            var allUsers = await userManager.Users.ToListAsync();
            foreach (var u in allUsers)
            {
                var mappedRole = NormalizeRoleName(u.Role);
                var currentRoles = await userManager.GetRolesAsync(u);

                if (u.Role != mappedRole || !currentRoles.Contains(mappedRole) || currentRoles.Count > 1)
                {
                    if (currentRoles.Count > 0)
                    {
                        await userManager.RemoveFromRolesAsync(u, currentRoles);
                    }
                    await userManager.AddToRoleAsync(u, mappedRole);
                    u.Role = mappedRole;
                    await userManager.UpdateAsync(u);
                }
            }

            // 3. Xóa các role cũ không nằm trong 3 role chuẩn nếu không còn user nào sử dụng
            var legacyRoleNames = new[] { "User", "Leader", "Kế toán" };
            foreach (var legacyName in legacyRoleNames)
            {
                var legacyRole = await roleManager.FindByNameAsync(legacyName);
                if (legacyRole != null)
                {
                    await roleManager.DeleteAsync(legacyRole);
                }
            }

            // 4. Seed tài khoản Admin mặc định
            await EnsureSeedUserAsync(
                userManager,
                userName: "admin",
                email: "admin@gmail.com",
                displayName: "Administrator",
                code: "ADMIN",
                role: RoleConstants.Admin,
                password: "Admin@1234",
                addAdminClaims: true);

            // 5. Seed tài khoản Accountant mẫu (gắn với Kế Toán Trưởng EMP009)
            await EnsureSeedUserAsync(
                userManager,
                userName: "accountant",
                email: "huy.bq@company.com",
                displayName: "Bùi Quang Huy",
                code: "EMP009",
                role: RoleConstants.Accountant,
                password: "Accountant@1234",
                addAdminClaims: false);

            // 6. Seed tài khoản Employee mẫu (gắn với Nhân viên EMP003)
            await EnsureSeedUserAsync(
                userManager,
                userName: "employee",
                email: "viet.lh@company.com",
                displayName: "Lê Hoàng Việt",
                code: "EMP003",
                role: RoleConstants.Employee,
                password: "Employee@1234",
                addAdminClaims: false);
        }

        private static string NormalizeRoleName(string? rawRole)
        {
            var lower = (rawRole ?? string.Empty).Trim().ToLowerInvariant();
            return lower switch
            {
                "admin" => RoleConstants.Admin,
                "accountant" or "kế toán" or "ke toan" => RoleConstants.Accountant,
                _ => RoleConstants.Employee
            };
        }

        private static async Task EnsureSeedUserAsync(
            UserManager<AppUser> userManager,
            string userName,
            string email,
            string displayName,
            string code,
            string role,
            string password,
            bool addAdminClaims)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                return;
            }

            var newUser = new AppUser
            {
                UserName = userName,
                Email = email,
                DisplayName = displayName,
                Role = role,
                Code = code,
                EmailConfirmed = true
            };

            var createRes = await userManager.CreateAsync(newUser, password);
            if (!createRes.Succeeded)
            {
                return;
            }

            await userManager.AddToRoleAsync(newUser, role);

            if (addAdminClaims)
            {
                var claims = await userManager.GetClaimsAsync(newUser);
                if (!claims.Any(c => c.Type == "permission" && c.Value == "FULL.ACCESS"))
                {
                    await userManager.AddClaimAsync(newUser, new Claim("permission", "FULL.ACCESS"));
                }
                if (!claims.Any(c => c.Type == "permission" && c.Value == "ASSET.VIEW"))
                {
                    await userManager.AddClaimAsync(newUser, new Claim("permission", "ASSET.VIEW"));
                }
            }
        }
    }
}
