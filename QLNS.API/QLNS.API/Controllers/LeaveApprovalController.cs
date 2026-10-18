using Microsoft.AspNetCore.Mvc;
using QLNS.Core.Interfaces.IService;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces;
using QLNSCore.Entities;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace QLNS.API.Controllers
{
    [Authorize(Roles ="Admin, Leader")]
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveApprovalController : ControllerBase
    {
        private readonly ILeaveApprovalService _service;

        public LeaveApprovalController(ILeaveApprovalService service)
        {
            _service = service;
        }

        [HttpGet("pending")]
        public async Task<ActionResult> GetPending(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var data = await _service.GetPendingApprovalsAsync(pageIndex, pageSize);
            return Ok(Result<PagedResult<LeaveRequestDto>>.Success(data));
        }

        [HttpPost("{id}/approve")]
        public async Task<ActionResult> Approve(Guid id)
        {
            try
            {
                var approvedBy = User.FindFirst(ClaimTypes.GivenName)?.Value ?? "Admin";
                var data = await _service.ApproveAsync(id, approvedBy);
                return Ok(Result<LeaveRequestDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(Result<string>.Failure(ex.Message));
            }
        }

        [HttpPost("{id}/reject")]
        public async Task<ActionResult> Reject(Guid id)
        {
            try
            {
                var rejectedBy = User.FindFirst(ClaimTypes.GivenName)?.Value ?? "Admin";
                var data = await _service.RejectAsync(id, rejectedBy);
                return Ok(Result<LeaveRequestDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(Result<string>.Failure(ex.Message));
            }
        }
    }
}
