using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces.IService;
using QLNSCore.Entities;
using System;
using System.Threading.Tasks;

namespace QLNS.API.Controllers
{
    [Authorize(Roles = "Admin, Leader")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportController(IReportService service)
        {
            _service = service;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult> GetDashboard()
        {
            var data = await _service.GetDashboardAsync();
            return Ok(Result<DashboardDto>.Success(data));
        }

        [HttpGet("employee-stats")]
        public async Task<ActionResult> GetEmployeeStatsByDepartment()
        {
            var items = await _service.GetEmployeeStatsByDepartmentAsync();
            var data = new ListItems<EmployeeStatsDto> { Items = items };
            return Ok(Result<ListItems<EmployeeStatsDto>>.Success(data));
        }

        [HttpGet("leave-report")]
        public async Task<ActionResult> GetLeaveReport(     
            [FromQuery] DateOnly fromDate,
            [FromQuery] DateOnly toDate)
        {
            var data = await _service.GetLeaveReportAsync(fromDate, toDate);
            return Ok(Result<LeaveReportDto>.Success(data));
        }

        [HttpGet("salary-report")]
        public async Task<ActionResult> GetSalaryReport(
            [FromQuery] int month,
            [FromQuery] int year)
        {
            var data = await _service.GetSalaryReportAsync(month, year);
            return Ok(Result<SalaryReportDto>.Success(data));
        }

        // Endpoint cảnh báo nhân sự: hợp đồng sắp hết hạn, sinh nhật trong tháng, sắp hết thời gian thử việc
        [HttpGet("hr-alerts")]
        public async Task<ActionResult> GetHrAlerts()
        {
            var data = await _service.GetHrAlertsAsync();
            return Ok(Result<HrAlertsDto>.Success(data));
        }
    }
}
