using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNS.API.Extension;
using QLNS.Core.Common;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces.IService;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LifecycleController : ControllerBase
    {
        private readonly ILifecycleService _service;

        public LifecycleController(ILifecycleService service)
        {
            _service = service;
        }

        [Authorize(Roles = RoleConstants.LifecycleViewAllRoles)]
        [HttpGet("overview")]
        public async Task<ActionResult> GetCompanyOverview([FromQuery] int? month = null, [FromQuery] int? year = null)
        {
            var now = DateTime.Now;
            var targetMonth = month ?? now.Month;
            var targetYear = year ?? now.Year;

            var data = await _service.GetCompanyOverviewAsync(targetMonth, targetYear);
            return Ok(Result<CompanyLifecycleOverviewDto>.Success(data));
        }

        [Authorize(Roles = RoleConstants.AllRoles)]
        [HttpGet("me")]
        public async Task<ActionResult> GetMyLifecycle([FromQuery] int? month = null, [FromQuery] int? year = null)
        {
            var now = DateTime.Now;
            var targetMonth = month ?? now.Month;
            var targetYear = year ?? now.Year;

            var userIdentifier = User.GetUserIdentifier();
            if (string.IsNullOrEmpty(userIdentifier))
                return Unauthorized(Result<string>.Failure("Không xác định được danh tính người dùng."));

            try
            {
                var data = await _service.GetMyLifecycleAsync(userIdentifier, targetMonth, targetYear);
                return Ok(Result<EmployeeLifecycleDetailDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = RoleConstants.LifecycleViewAllRoles)]
        [HttpGet("{employeeId}")]
        public async Task<ActionResult> GetEmployeeLifecycle(Guid employeeId, [FromQuery] int? month = null, [FromQuery] int? year = null)
        {
            var now = DateTime.Now;
            var targetMonth = month ?? now.Month;
            var targetYear = year ?? now.Year;

            try
            {
                var data = await _service.GetEmployeeLifecycleAsync(employeeId, targetMonth, targetYear);
                return Ok(Result<EmployeeLifecycleDetailDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }
    }
}
