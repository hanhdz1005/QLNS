using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLNS.Core.Common;
using QLNSCore.Entities;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QLNS.Infrastructure.Data.Config
{
    public class IdentitySeed
    {
        public static async Task EnsureExtendedSchemaAsync(AppDbContext dbContext)
        {
            const string sql = @"
IF COL_LENGTH('ATTENDANCES', 'RegularHours') IS NULL
BEGIN
    ALTER TABLE [ATTENDANCES] ADD [RegularHours] decimal(5,2) NULL DEFAULT 0;
END

IF COL_LENGTH('ATTENDANCES', 'OvertimeHours') IS NULL
BEGIN
    ALTER TABLE [ATTENDANCES] ADD [OvertimeHours] decimal(5,2) NULL DEFAULT 0;
END

IF COL_LENGTH('ATTENDANCES', 'ScanCount') IS NULL
BEGIN
    ALTER TABLE [ATTENDANCES] ADD [ScanCount] int NOT NULL DEFAULT 1;
END

IF OBJECT_ID('OVERTIME_REQUESTS', 'U') IS NULL
BEGIN
    CREATE TABLE [OVERTIME_REQUESTS] (
        [Id] uniqueidentifier NOT NULL PRIMARY KEY,
        [EmployeeId] uniqueidentifier NOT NULL,
        [EmployeeName] nvarchar(200) NOT NULL,
        [Date] date NOT NULL,
        [RegisteredHours] decimal(5,2) NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [Status] int NOT NULL,
        [ApprovedBy] nvarchar(200) NULL,
        [CreatedDate] date NOT NULL,
        CONSTRAINT [FK_OVERTIME_REQUESTS_EMPLOYEES_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [EMPLOYEES] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_OVERTIME_REQUESTS_EmployeeId_Date] ON [OVERTIME_REQUESTS] ([EmployeeId], [Date]);
END

IF NOT EXISTS (SELECT 1 FROM [OVERTIME_REQUESTS]) AND EXISTS (SELECT 1 FROM [EMPLOYEES])
BEGIN
    DECLARE @EmpId uniqueidentifier;
    DECLARE @EmpName nvarchar(200);
    SELECT TOP 1 @EmpId = [Id], @EmpName = [FullName] FROM [EMPLOYEES] ORDER BY [Code];
    IF @EmpId IS NOT NULL
    BEGIN
        INSERT INTO [OVERTIME_REQUESTS] ([Id], [EmployeeId], [EmployeeName], [Date], [RegisteredHours], [Reason], [Status], [ApprovedBy], [CreatedDate])
        VALUES
        (NEWID(), @EmpId, @EmpName, CAST(GETDATE() AS date), 2.5, N'Triển khai bản phát hành hệ thống HRMS Pro', 1, NULL, CAST(GETDATE() AS date)),
        (NEWID(), @EmpId, @EmpName, DATEADD(day, -2, CAST(GETDATE() AS date)), 3.0, N'Đối soát dữ liệu chấm công và quyết toán cuối tháng', 2, N'Trần Đức Nam (Dept Manager)', DATEADD(day, -2, CAST(GETDATE() AS date)));
    END
END
";
            try
            {
                await dbContext.Database.ExecuteSqlRawAsync(sql);
            }
            catch
            {
                // Ignore if DB is not yet migrated or using non-SQL Server provider
            }
        }

        public static async Task SeedAdminAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Đảm bảo 6 role phân tầng doanh nghiệp luôn tồn tại
            foreach (var roleName in RoleConstants.CoreRoles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Đồng bộ các tài khoản cũ sang hệ thống 6 role chuẩn
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

            // 3. Xóa các role cũ không nằm trong danh sách chuẩn
            var legacyRoleNames = new[] { "User", "Leader", "Kế toán" };
            foreach (var legacyName in legacyRoleNames)
            {
                var legacyRole = await roleManager.FindByNameAsync(legacyName);
                if (legacyRole != null)
                {
                    await roleManager.DeleteAsync(legacyRole);
                }
            }

            // 4. Seed tài khoản cho 6 tầng vai trò
            await EnsureSeedUserAsync(
                userManager,
                userName: "admin",
                email: "admin@gmail.com",
                displayName: "Administrator",
                code: "EMP001",
                role: RoleConstants.Admin,
                password: "Admin@1234",
                addAdminClaims: true);

            await EnsureSeedUserAsync(
                userManager,
                userName: "hrmanager",
                email: "hrmanager@company.com",
                displayName: "Đỗ Thu Hà (HR Manager)",
                code: "EMP002",
                role: RoleConstants.HRManager,
                password: "HrManager@1234",
                addAdminClaims: false);

            await EnsureSeedUserAsync(
                userManager,
                userName: "hrstaff",
                email: "hrstaff@company.com",
                displayName: "Phạm Thị Mai (HR Staff)",
                code: "EMP005",
                role: RoleConstants.HRStaff,
                password: "HrStaff@1234",
                addAdminClaims: false);

            await EnsureSeedUserAsync(
                userManager,
                userName: "accountant",
                email: "huy.bq@company.com",
                displayName: "Bùi Quang Huy",
                code: "EMP009",
                role: RoleConstants.Accountant,
                password: "Accountant@1234",
                addAdminClaims: false);

            await EnsureSeedUserAsync(
                userManager,
                userName: "manager",
                email: "manager@company.com",
                displayName: "Trần Đức Nam (Dept Manager)",
                code: "EMP004",
                role: RoleConstants.DepartmentManager,
                password: "Manager@1234",
                addAdminClaims: false);

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

        public static string NormalizeRoleName(string? rawRole)
        {
            var lower = (rawRole ?? string.Empty).Trim().ToLowerInvariant();
            return lower switch
            {
                "admin" => RoleConstants.Admin,
                "hrmanager" or "hr_manager" or "hr manager" => RoleConstants.HRManager,
                "hrstaff" or "hr_staff" or "hr staff" or "hr" => RoleConstants.HRStaff,
                "accountant" or "kế toán" or "ke toan" => RoleConstants.Accountant,
                "departmentmanager" or "department_manager" or "manager" or "leader" or "trưởng phòng" => RoleConstants.DepartmentManager,
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
                if (string.IsNullOrWhiteSpace(existing.Code) || existing.Code == "ADMIN")
                {
                    existing.Code = code;
                    await userManager.UpdateAsync(existing);
                }
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
