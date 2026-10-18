using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLNS.API.Controllers
{
    [Authorize(Roles ="Admin")]
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
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(Result<ListItems<IdentityRole>>.Success(new ListItems<IdentityRole> { Items = roles }));
        }

        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest(Result<bool>.Failure("Role name is required"));

            var exists = await _roleManager.RoleExistsAsync(roleName);
            if (exists)
                return BadRequest(Result<bool>.Failure("Role already exists"));

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
                return BadRequest(Result<bool>.Failure(string.Join(", ", result.Errors.Select(e => e.Description))));

            return Ok(Result<IdentityRole>.Success(new IdentityRole(roleName)));
        }

        [HttpDelete("{roleName}")]
        public async Task<ActionResult> DeleteRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound(Result<bool>.Failure("Role not found"));

            if (role.Name == "Admin" || role.Name == "User")
                return BadRequest(Result<bool>.Failure("Cannot delete default roles"));

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
                Role = u.Role
            }).ToList();
            return Ok(Result<ListItems<UserInfoDto>>.Success(new ListItems<UserInfoDto> { Items = userDtos }));
        }

        [HttpPost("assign-role")]
        public async Task<ActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound(Result<bool>.Failure("User not found"));

            var result = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!result.Succeeded)
                return BadRequest(Result<bool>.Failure(string.Join(", ", result.Errors.Select(e => e.Description))));

            return Ok(Result<bool>.Success(true));
        }

        [HttpPut("user-role")]
        public async Task<ActionResult> UpdateUserRole(Guid Id, [FromBody] string assignRole)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            if (user == null)
                return NotFound(Result<bool>.Failure("User not found"));

            var role = await _roleManager.FindByNameAsync(assignRole);
            if (role == null)
                return NotFound(Result<bool>.Failure("Role not found"));

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return BadRequest(Result<bool>.Failure(string.Join(", ", removeResult.Errors.Select(e => e.Description))));

            var addResult = await _userManager.AddToRoleAsync(user, assignRole);
            if (!addResult.Succeeded)
                return BadRequest(Result<bool>.Failure(string.Join(", ", addResult.Errors.Select(e => e.Description))));

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
                return  BadRequest(Result<bool>.Failure(string.Join(", ", result.Errors.Select(e => e.Description))));

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
