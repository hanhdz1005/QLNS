using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNS.Core.Common;
using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QLNS.API.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            return Ok(Result<ListItems<IdentityRole>>.Success(new ListItems<IdentityRole> { Items = roles }));
        }

        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest(Result<bool>.Failure("Role name is required"));

            var trimmed = roleName.Trim();
            var exists = await _roleManager.RoleExistsAsync(trimmed);
            if (exists)
                return BadRequest(Result<bool>.Failure("Role already exists"));

            var result = await _roleManager.CreateAsync(new IdentityRole(trimmed));
            if (!result.Succeeded)
                return BadRequest(Result<bool>.Failure(string.Join(", ", result.Errors.Select(e => e.Description))));

            var createdRole = await _roleManager.FindByNameAsync(trimmed);
            return Ok(Result<IdentityRole>.Success(createdRole!));
        }

        [HttpDelete("{roleName}")]
        public async Task<ActionResult> DeleteRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound(Result<bool>.Failure("Role not found"));

            if (RoleConstants.CoreRoles.Any(c => string.Equals(c, role.Name, StringComparison.OrdinalIgnoreCase)))
                return BadRequest(Result<bool>.Failure("Không thể xóa 3 vai trò mặc định của hệ thống (Admin, Accountant, Employee)."));

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                return BadRequest(Result<bool>.Failure(string.Join(", ", result.Errors.Select(e => e.Description))));

            return Ok(Result<bool>.Success(true));
        }

        [HttpGet("users")]
        public async Task<ActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = users.Select(u => new UserInfoDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                DisplayName = u.DisplayName,
                Role = string.IsNullOrWhiteSpace(u.Role) ? RoleConstants.Employee : u.Role
            }).ToList();
            return Ok(Result<ListItems<UserInfoDto>>.Success(new ListItems<UserInfoDto> { Items = userDtos }));
        }

        [HttpPost("assign-role")]
        public async Task<ActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound(Result<bool>.Failure("User not found"));

            var targetRole = await _roleManager.FindByNameAsync(dto.Role);
            if (targetRole == null || string.IsNullOrWhiteSpace(targetRole.Name))
                return NotFound(Result<bool>.Failure("Role not found"));

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Count > 0)
            {
                var removeRes = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRes.Succeeded)
                    return BadRequest(Result<bool>.Failure(string.Join(", ", removeRes.Errors.Select(e => e.Description))));
            }

            var addRes = await _userManager.AddToRoleAsync(user, targetRole.Name);
            if (!addRes.Succeeded)
                return BadRequest(Result<bool>.Failure(string.Join(", ", addRes.Errors.Select(e => e.Description))));

            user.Role = targetRole.Name;
            await _userManager.UpdateAsync(user);

            return Ok(Result<bool>.Success(true));
        }

        [HttpPut("user-role")]
        public async Task<ActionResult> UpdateUserRole(Guid Id, [FromBody] string assignRole)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            if (user == null)
                return NotFound(Result<bool>.Failure("User not found"));

            var role = await _roleManager.FindByNameAsync(assignRole);
            if (role == null || string.IsNullOrWhiteSpace(role.Name))
                return NotFound(Result<bool>.Failure("Role not found"));

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return BadRequest(Result<bool>.Failure(string.Join(", ", removeResult.Errors.Select(e => e.Description))));
            }

            var addResult = await _userManager.AddToRoleAsync(user, role.Name);
            if (!addResult.Succeeded)
                return BadRequest(Result<bool>.Failure(string.Join(", ", addResult.Errors.Select(e => e.Description))));

            user.Role = role.Name;
            await _userManager.UpdateAsync(user);

            return Ok(Result<bool>.Success(true));
        }

        [HttpPost("assign-claim")]
        public async Task<ActionResult> AssignClaim([FromBody] AssignClaimDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound(Result<bool>.Failure("User not found"));

            var claims = await _userManager.GetClaimsAsync(user);
            if (claims.Any(c => c.Type == "permission" && c.Value == dto.ClaimValue))
                return BadRequest(Result<bool>.Failure("User already has this claim"));

            var result = await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("permission", dto.ClaimValue));
            if (!result.Succeeded)
                return BadRequest(Result<bool>.Failure(string.Join(", ", result.Errors.Select(e => e.Description))));

            return Ok(Result<bool>.Success(true));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateRoleById(Guid Id, [FromBody] AddRolesDto dto)
        {
            var role = await _roleManager.FindByIdAsync(Id.ToString());
            if (role == null)
                return NotFound(Result<bool>.Failure("Role not found"));

            role.Name = dto.Name;
            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                return BadRequest(Result<bool>.Failure(string.Join(", ", result.Errors.Select(e => e.Description))));

            return Ok(Result<bool>.Success(true));
        }
    }
}
