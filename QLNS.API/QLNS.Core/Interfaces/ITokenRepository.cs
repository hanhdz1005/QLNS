using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces
{
    public interface ITokenRepository
    {
        string CreateToken(AppUser appUser);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
