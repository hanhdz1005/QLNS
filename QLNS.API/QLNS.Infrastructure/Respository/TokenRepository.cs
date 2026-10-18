using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QLNSCore.Entities;
using QLNS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Infrastructure.Respository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TokenRepository(IConfiguration config, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _config = config;

            _roleManager = roleManager;
            _userManager = userManager;

            var key = _config["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is missing");
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }
        public string CreateToken(AppUser appUser)
        {
            var role = appUser.Role;

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var Claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
                new Claim(JwtRegisteredClaimNames.PreferredUsername, appUser.UserName),
                new Claim(JwtRegisteredClaimNames.GivenName, appUser.DisplayName),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier, appUser.Id)
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.Now.AddDays(10),
                Issuer = _config["Jwt:Issuer"],
                SigningCredentials = creds,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_config["Jwt:Key"])),

                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out SecurityToken securityToken);

            return principal;
        }
    }
}
