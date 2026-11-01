using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLNS.API.Extension;
using QLNS.Core.Common;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces.IService;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QLNS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _service;
        private readonly UserManager<AppUser> _userManager;

        public AttendanceController(IAttendanceService service, UserManager<AppUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [Authorize(Roles = RoleConstants.AdminOrAccountant)]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            var data = new ListItems<AttendanceDto>
            {
                Items = items.ToList()
            };
            return Ok(Result<ListItems<AttendanceDto>>.Success(data));
        }

        [Authorize(Roles = RoleConstants.AdminOrAccountant)]
        [HttpGet("filter")]
        public async Task<ActionResult> GetFilter(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? search = null)
        {
            var data = await _service.GetFilterAsync(pageIndex, pageSize, search);
            return Ok(Result<PagedResult<AttendanceDto>>.Success(data));
        }

        [Authorize(Roles = RoleConstants.AdminOrAccountant)]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            try
            {
                var data = await _service.GetByIdAsync(id);
                return Ok(Result<AttendanceDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost]
        public async Task<ActionResult> Add(AddAttendanceDto dto)
        {
            try
            {
                var data = await _service.AddAsync(dto);
                return Ok(Result<AttendanceDto>.Success(data));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(Result<bool>.Failure(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, AddAttendanceDto dto)
        {
            try
            {
                var data = await _service.UpdateAsync(id, dto);
                return Ok(Result<AttendanceDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<bool>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(Result<bool>.Success(true));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<bool>.Failure(ex.Message));
            }
        }

        // B1: Đăng ký đặc điểm nhận diện khuôn mặt lần đầu khi đăng nhập
        [Authorize(Roles = RoleConstants.AllRoles)]
        [HttpPost("register-face")]
        public async Task<ActionResult> RegisterFace([FromBody] FaceRegisterDto dto)
        {
            var userIdentifier = User.GetUserIdentifier();
            if (string.IsNullOrEmpty(userIdentifier))
                return Unauthorized(Result<string>.Failure("Không xác định được danh tính người dùng.", errorCode: "UNAUTHORIZED"));

            var (success, message, errorCode, imageUrl) = await _service.RegisterFaceAsync(userIdentifier, dto);
            if (!success)
            {
                return BadRequest(Result<string>.Failure(message, errorCode: errorCode ?? "FACE_REGISTER_FAILED"));
            }

            var email = User.FindFirstValue(ClaimTypes.Email) ?? userIdentifier;
            var appUser = await _userManager.FindByEmailAsync(email);
            if (appUser != null)
            {
                appUser.FaceEmbedding = FaceRecognitionHelper.SerializeDescriptor(dto.FaceDescriptor);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    appUser.AvatarUrl = imageUrl;
                }
                appUser.FaceRegisteredAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(appUser);
            }

            return Ok(Result<object>.Success(new
            {
                hasRegisteredFace = true,
                faceImageUrl = imageUrl
            }));
        }

        // B3 & B4: Gửi ảnh/đặc điểm khuôn mặt về BE đối sánh với DB để chấm công
        [Authorize(Roles = RoleConstants.AllRoles)]
        [HttpPost("face-checkin")]
        public async Task<ActionResult> FaceCheckIn([FromBody] FaceCheckInDto dto)
        {
            var userIdentifier = User.GetUserIdentifier();
            if (string.IsNullOrEmpty(userIdentifier))
                return Unauthorized(Result<string>.Failure("Không xác định được danh tính người dùng đăng nhập.", errorCode: "UNAUTHORIZED"));

            var email = User.FindFirstValue(ClaimTypes.Email) ?? userIdentifier;
            var appUser = await _userManager.FindByEmailAsync(email);

            var result = await _service.FaceCheckInAsync(userIdentifier, dto, appUser?.FaceEmbedding);
            if (!result.Success)
            {
                return BadRequest(Result<FaceCheckInResultDto>.Failure(
                    result.Message,
                    errorCode: result.ErrorCode ?? "FACE_MISMATCH"));
            }

            return Ok(Result<FaceCheckInResultDto>.Success(result));
        }

        // B2 & B4: Ghi nhận và trả về Error Code khi kết nối Camera thất bại
        [Authorize(Roles = RoleConstants.AllRoles)]
        [HttpPost("camera-error")]
        public ActionResult ReportCameraError([FromBody] CameraErrorReportDto dto)
        {
            var code = string.IsNullOrWhiteSpace(dto?.ErrorCode) ? "CAMERA_CONNECT_FAILED" : dto.ErrorCode;
            var detail = string.IsNullOrWhiteSpace(dto?.ErrorDetail)
                ? "Không thể kết nối với thiết bị Camera của người dùng."
                : dto.ErrorDetail;

            return BadRequest(Result<object>.Failure(
                message: detail,
                errors: new List<string> { $"Camera Error [{code}]: {detail}" },
                errorCode: code));
        }

        // Legacy QR Endpoints
        [AllowAnonymous]
        [HttpGet("qr-token")]
        public async Task<ActionResult> GetQrToken()
        {
            var (token, expiresInSeconds) = await _service.GenerateQrTokenAsync();
            return Ok(Result<object>.Success(new
            {
                token,
                expiresInSeconds,
                generatedAt = DateTime.UtcNow
            }));
        }

        [Authorize(Roles = RoleConstants.AllRoles)]
        [HttpPost("qr-checkin")]
        public async Task<ActionResult> QrCheckIn([FromBody] QrCheckInDto dto)
        {
            var userIdentifier = User.GetUserIdentifier();
            if (string.IsNullOrEmpty(userIdentifier))
                return Unauthorized(Result<string>.Failure("Không xác định được danh tính người dùng đăng nhập.", errorCode: "UNAUTHORIZED"));

            var result = await _service.QrCheckInAsync(dto.QrToken, userIdentifier);
            if (!result.Success)
            {
                return BadRequest(Result<QrCheckInResultDto>.Failure(result.Message));
            }

            return Ok(Result<QrCheckInResultDto>.Success(result));
        }

        [Authorize(Roles = RoleConstants.AllRoles)]
        [HttpGet("my-attendance")]
        public async Task<ActionResult> GetMyAttendance([FromQuery] int? month = null, [FromQuery] int? year = null)
        {
            var now = DateTime.UtcNow;
            var targetMonth = month ?? now.Month;
            var targetYear = year ?? now.Year;

            var userIdentifier = User.GetUserIdentifier();
            if (string.IsNullOrEmpty(userIdentifier))
                return Unauthorized(Result<string>.Failure("Không xác định được danh tính người dùng.", errorCode: "UNAUTHORIZED"));

            try
            {
                var list = await _service.GetMyAttendanceAsync(userIdentifier, targetMonth, targetYear);
                return Ok(Result<IEnumerable<AttendanceDto>>.Success(list));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }
    }
}
