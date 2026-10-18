using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces;
using QLNSCore.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QLNS.Infrastructure.Respository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenRepository _tokenRepository;

        public AuthRepository(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenRepository = tokenRepository;
        }

        public async Task<TokenDto> RegisterAsync(string displayName, string userName, string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                throw new InvalidOperationException("Email already exists");

            var existingUsername = await _userManager.FindByNameAsync(userName);
            if (existingUsername != null)
                throw new InvalidOperationException("Username already exists");

            var user = new AppUser
            {
                DisplayName = displayName,
                UserName = userName,
                Email = email,
                Role = "User",
                Code = userName.ToUpper()
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new InvalidOperationException(string.Join(", ", errors));
            }

            await _userManager.AddToRoleAsync(user, "User");

            var accessToken = _tokenRepository.CreateToken(user);
            var refreshToken = _tokenRepository.GenerateRefreshToken();

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<TokenDto> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new KeyNotFoundException("Invalid email or password");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                throw new KeyNotFoundException("Invalid email or password");

            var accessToken = _tokenRepository.CreateToken(user);
            var refreshToken = _tokenRepository.GenerateRefreshToken();

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var principal = _tokenRepository.GetPrincipalFromExpiredToken(accessToken);
            var email = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var newAccessToken = _tokenRepository.CreateToken(user);
            var newRefreshToken = _tokenRepository.GenerateRefreshToken();

            return new AuthResultDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Role = user.Role
            };
        }

       
    }
}
