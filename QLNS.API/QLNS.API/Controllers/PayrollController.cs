using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces.IService;
using QLNSCore.Entities;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using QLNS.API.Extension;

namespace QLNS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollService _service;

        public PayrollController(IPayrollService service)
        {
            _service = service;
        }

        [Authorize(Roles = QLNS.Core.Common.RoleConstants.AdminOrAccountant)]
        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] int month, [FromQuery] int year)
        {
            var items = await _service.GetAllAsync(month, year);
            var data = new ListItems<PayrollDto>
            {
                Items = items.ToList()
            };
            return Ok(Result<ListItems<PayrollDto>>.Success(data));
        }

        [Authorize(Roles = QLNS.Core.Common.RoleConstants.AdminOrAccountant)]
        [HttpGet("generate")]
        public async Task<ActionResult> GeneratePayroll(
            [FromQuery] Guid employeeId,
            [FromQuery] int month,
            [FromQuery] int year,
            [FromQuery] bool saveToDb = true)
        {
            try
            {
                var data = await _service.GeneratePayrollAsync(employeeId, month, year, saveToDb);
                return Ok(Result<PayrollDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }

        [Authorize(Roles = QLNS.Core.Common.RoleConstants.AdminOrAccountant)]
        [HttpPost("generate-bulk")]
        public async Task<ActionResult> GenerateBulkPayroll([FromBody] PayrollGenerateDto dto)
        {
            var items = await _service.GenerateBulkPayrollAsync(dto);
            var data = new ListItems<PayrollDto>
            {
                Items = items.ToList()
            };
            return Ok(Result<ListItems<PayrollDto>>.Success(data));
        }

        // Endpoint xem phiếu lương cá nhân của nhân viên (Employee Self-Service)
        [Authorize(Roles = QLNS.Core.Common.RoleConstants.AllRoles)]
        [HttpGet("my-payslip")]
        public async Task<ActionResult> GetMyPayslip([FromQuery] int? month = null, [FromQuery] int? year = null)
        {
            var now = DateTime.UtcNow;
            var targetMonth = month ?? now.Month;
            var targetYear = year ?? now.Year;

            var userIdentifier = User.GetUserIdentifier();

            if (string.IsNullOrEmpty(userIdentifier))
                return Unauthorized(Result<string>.Failure("Không xác định được danh tính người dùng.", errorCode: "UNAUTHORIZED"));

            try
            {
                var data = await _service.GetMyPayslipAsync(userIdentifier, targetMonth, targetYear);
                return Ok(Result<PayrollDto>.Success(data));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Result<string>.Failure(ex.Message));
            }
        }
    }
}
