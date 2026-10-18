using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLNS.Core.Interfaces.IService;
using QLNS.Core.Dto;
using QLNSCore.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QLNS.API.Extension;

namespace QLNS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _service;

        public AttendanceController(IAttendanceService service)
        {
            _service = service;
        }

        [Authorize(Roles = "User, Admin, Leader")]
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

        [Authorize(Roles = "User, Admin, Leader")]
        [HttpGet("filter")]
        public async Task<ActionResult> GetFilter(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? search = null)
        {
            var data = await _service.GetFilterAsync(pageIndex, pageSize, search);
            return Ok(Result<PagedResult<AttendanceDto>>.Success(data));
        }

        [Authorize(Roles = "User, Admin, Leader")]
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

        [Authorize(Roles = "User, Admin, Leader")]
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

        // Endpoint cho Kiosk/Tablet hiển thị mã QR động (reset mỗi 30s)
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

        // Endpoint cho nhân viên quét mã QR trên điện thoại để điểm danh Vào/Ra
        [Authorize(Roles = "User, Admin, Leader")]
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

        // Endpoint xem bảng chấm công cá nhân của nhân viên (Employee Self-Service)
        [Authorize(Roles = "User, Admin, Leader")]
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