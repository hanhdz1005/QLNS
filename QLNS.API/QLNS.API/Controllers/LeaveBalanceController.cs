using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces.IService;
using QLNSCore.Entities;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using QLNS.API.Extension;

namespace QLNS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveBalanceController : ControllerBase
    {
        private readonly ILeaveBalanceService _leaveBalanceService;

        public LeaveBalanceController(ILeaveBalanceService leaveBalanceService)
        {
            _leaveBalanceService = leaveBalanceService;
        }

        // Endpoint xem số ngày phép còn lại của nhân viên đang đăng nhập (Employee Self-Service)
        [Authorize(Roles = QLNS.Core.Common.RoleConstants.AllRoles)]
        [HttpGet("my-balance")]
        public async Task<ActionResult> GetMyBalance([FromQuery] int? year = null)
        {
            var targetYear = year ?? DateTime.UtcNow.Year;
            var userIdentifier = User.GetUserIdentifier();

            if (string.IsNullOrEmpty(userIdentifier))
                return Unauthorized(Result<string>.Failure("Không xác định được danh tính người dùng.", errorCode: "UNAUTHORIZED"));

            try
            {
                var data = await _leaveBalanceService.GetMyBalanceAsync(userIdentifier, targetYear);
                return Ok(Result<LeaveBalanceDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }

        // Endpoint tra cứu phép năm của nhân viên theo Id (HR / Kế toán)
        [Authorize(Roles = QLNS.Core.Common.RoleConstants.AdminOrAccountant)]
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult> GetByEmployeeId(Guid employeeId, [FromQuery] int? year = null)
        {
            var targetYear = year ?? DateTime.UtcNow.Year;
            try
            {
                var data = await _leaveBalanceService.GetByEmployeeAndYearAsync(employeeId, targetYear);
                return Ok(Result<LeaveBalanceDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }

        // Điều chỉnh tổng số ngày phép năm cho nhân viên (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPut("update-total")]
        public async Task<ActionResult> UpdateTotalDays([FromBody] UpdateLeaveBalanceDto dto)
        {
            try
            {
                var data = await _leaveBalanceService.UpdateTotalDaysAsync(dto.EmployeeId, dto.Year, dto.TotalLeaveDays);
                return Ok(Result<LeaveBalanceDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }
    }
}
