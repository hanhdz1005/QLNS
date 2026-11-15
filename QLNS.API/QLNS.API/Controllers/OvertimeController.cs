using Microsoft.AspNetCore.Authorization;
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
    public class OvertimeController : ControllerBase
    {
        private readonly IOvertimeService _service;

        public OvertimeController(IOvertimeService service)
        {
            _service = service;
        }

        [Authorize(Roles = RoleConstants.AllRoles)]
        [HttpGet("my")]
        public async Task<ActionResult> GetMyRequests()
        {
            var userIdentifier = User.GetUserIdentifier();
            if (string.IsNullOrEmpty(userIdentifier))
                return Unauthorized(Result<string>.Failure("Không xác định được danh tính người dùng."));

            var items = await _service.GetMyRequestsAsync(userIdentifier);
            return Ok(Result<ListItems<OvertimeRequestDto>>.Success(new ListItems<OvertimeRequestDto>
            {
                Items = items.ToList()
            }));
        }

        [Authorize(Roles = RoleConstants.AttendanceViewAllRoles)]
        [HttpGet]
        public async Task<ActionResult> GetAllRequests()
        {
            var items = await _service.GetAllRequestsAsync();
            return Ok(Result<ListItems<OvertimeRequestDto>>.Success(new ListItems<OvertimeRequestDto>
            {
                Items = items.ToList()
            }));
        }

        [Authorize(Roles = RoleConstants.ApproverRoles)]
        [HttpGet("pending")]
        public async Task<ActionResult> GetPendingApprovals()
        {
            var items = await _service.GetPendingApprovalsAsync();
            return Ok(Result<ListItems<OvertimeRequestDto>>.Success(new ListItems<OvertimeRequestDto>
            {
                Items = items.ToList()
            }));
        }

        [Authorize(Roles = RoleConstants.AllRoles)]
        [HttpGet("summary")]
        public async Task<ActionResult> GetMonthlySummary([FromQuery] int? month = null, [FromQuery] int? year = null, [FromQuery] bool companyWide = false)
        {
            var userIdentifier = User.GetUserIdentifier();
            if (string.IsNullOrEmpty(userIdentifier))
                return Unauthorized(Result<string>.Failure("Không xác định được danh tính người dùng."));

            var now = DateTime.Now;
            var targetMonth = month ?? now.Month;
            var targetYear = year ?? now.Year;

            var summary = await _service.GetMonthlySummaryAsync(userIdentifier, targetMonth, targetYear, companyWide);
            return Ok(Result<OvertimeMonthlySummaryDto>.Success(summary));
        }

        [Authorize(Roles = RoleConstants.AllRoles)]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] AddOvertimeRequestDto dto)
        {
            var userIdentifier = User.GetUserIdentifier();
            if (string.IsNullOrEmpty(userIdentifier))
                return Unauthorized(Result<string>.Failure("Không xác định được danh tính người dùng."));

            try
            {
                var data = await _service.CreateAsync(userIdentifier, dto);
                return Ok(Result<OvertimeRequestDto>.Success(data));
            }
            catch (Exception ex)
            {
                return BadRequest(Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = RoleConstants.ApproverRoles)]
        [HttpPost("{id}/approve")]
        public async Task<ActionResult> Approve(Guid id, [FromBody] ReviewOvertimeRequestDto? dto = null)
        {
            try
            {
                var approvedBy = User.FindFirst(ClaimTypes.GivenName)?.Value
                    ?? User.GetUserIdentifier()
                    ?? "DepartmentManager";
                var data = await _service.ApproveAsync(id, approvedBy, dto);
                return Ok(Result<OvertimeRequestDto>.Success(data));
            }
            catch (Exception ex)
            {
                return BadRequest(Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = RoleConstants.ApproverRoles)]
        [HttpPost("{id}/reject")]
        public async Task<ActionResult> Reject(Guid id, [FromBody] ReviewOvertimeRequestDto? dto = null)
        {
            try
            {
                var rejectedBy = User.FindFirst(ClaimTypes.GivenName)?.Value
                    ?? User.GetUserIdentifier()
                    ?? "DepartmentManager";
                var data = await _service.RejectAsync(id, rejectedBy, dto);
                return Ok(Result<OvertimeRequestDto>.Success(data));
            }
            catch (Exception ex)
            {
                return BadRequest(Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = RoleConstants.AllRoles)]
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> Cancel(Guid id)
        {
            var userIdentifier = User.GetUserIdentifier();
            if (string.IsNullOrEmpty(userIdentifier))
                return Unauthorized(Result<string>.Failure("Không xác định được danh tính người dùng."));

            try
            {
                var data = await _service.CancelAsync(id, userIdentifier);
                return Ok(Result<OvertimeRequestDto>.Success(data));
            }
            catch (Exception ex)
            {
                return BadRequest(Result<string>.Failure(ex.Message));
            }
        }
    }
}
