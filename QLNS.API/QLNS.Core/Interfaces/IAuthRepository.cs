using System.Threading.Tasks;
using QLNS.Core.Dto;


namespace QLNS.Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<TokenDto> RegisterAsync(string displayName, string userName, string email, string password);
        Task<TokenDto> LoginAsync(string email, string password);
        Task<AuthResultDto> RefreshTokenAsync(string accessToken, string refreshToken);
    }

    public class AuthResultDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
   
}
