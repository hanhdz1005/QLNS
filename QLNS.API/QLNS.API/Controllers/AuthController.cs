using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces;
using QLNSCore.Entities;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QLNS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmployeeRepository _employeeRepository;

        public AuthController(
            IAuthRepository authRepository,
            UserManager<AppUser> userManager,
            IEmployeeRepository employeeRepository)
        {
            _authRepository = authRepository;
            _userManager = userManager;
            _employeeRepository = employeeRepository;
        }

        private async Task<bool> CheckEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto dto)
        {
            var isCheckEmail = await CheckEmail(dto.Email);
            if (!isCheckEmail) return BadRequest(Result<bool>.Failure("Email is already in user"));

            try
            {
                var data = await _authRepository.RegisterAsync(dto.DisplayName, dto.UserName, dto.Email, dto.Password);
                return Ok(Result<TokenDto>.Success(data));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(Result<bool>.Failure(ex.Message));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto dto)
        {
            try
            {
                var data = await _authRepository.LoginAsync(dto.Email, dto.Password);
                return Ok(Result<TokenDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(Result<string>.Failure(ex.Message));
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            try
            {
                var data = await _authRepository.RefreshTokenAsync(dto.AccessToken, dto.RefreshToken);
                return Ok(Result<AuthResultDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }

        [HttpGet("getCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
                return Unauthorized(Result<bool>.Failure("Không xác định được danh tính người dùng.", errorCode: "UNAUTHORIZED"));

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Unauthorized(Result<bool>.Failure("Người dùng không tồn tại.", errorCode: "USER_NOT_FOUND"));

            var employee = await _employeeRepository.GetByIdentifierAsync(email);
            bool hasRegisteredFace = !string.IsNullOrWhiteSpace(user.FaceEmbedding)
                || !string.IsNullOrWhiteSpace(employee?.FaceEmbedding);

            var normalizedRole = string.IsNullOrWhiteSpace(user.Role) || user.Role == "User" || user.Role == "Leader"
                ? QLNS.Core.Common.RoleConstants.Employee
                : user.Role;

            var data = new GetCurrentUserDto
            {
                Id = Guid.Parse(user.Id),
                DisplayName = user.DisplayName,
                Role = normalizedRole,
                HasRegisteredFace = hasRegisteredFace,
                FaceImageUrl = user.AvatarUrl ?? employee?.FaceImageUrl
            };

            return Ok(Result<GetCurrentUserDto>.Success(data));
        }
    }
}
