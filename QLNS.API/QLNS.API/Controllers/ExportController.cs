using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces.IService;
using QLNSCore.Entities;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IEmployeeService _employeeService;

        public ExportController(IReportService reportService, IEmployeeService employeeService)
        {
            _reportService = reportService;
            _employeeService = employeeService;
        }

        [HttpGet("employee-csv")]
        public async Task<ActionResult> ExportEmployeeCsv()
        {
            var employees = await _employeeService.GetAllAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Id,Code,FullName,DateOfBirth,Gender,PhoneNumber,Email,Address,Department,Position,JoinDate,Status");

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.Id},{emp.Code},{emp.FullName},{emp.DateOfBirth},{emp.Gender},{emp.PhoneNumber},{emp.Email},{emp.Address},{emp.DepartmentName},{emp.PositionName},{emp.JoinDate},{emp.Status}");
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"employees_{DateTime.Now:yyyyMMdd}.csv");
        }

        [HttpGet("salary-csv")]
        public async Task<ActionResult> ExportSalaryCsv([FromQuery] int month, [FromQuery] int year)
        {
            var report = await _reportService.GetSalaryReportAsync(month, year);

            var sb = new StringBuilder();
            sb.AppendLine("Month,Year,TotalRecords,TotalBasicSalary,TotalNetSalary,AverageNetSalary,MaxSalary,MinSalary");
            sb.AppendLine($"{month},{year},{report.TotalRecords},{report.TotalBasicSalary},{report.TotalNetSalary},{report.AverageNetSalary},{report.MaxSalary},{report.MinSalary}");

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"salary_report_{month}_{year}.csv");
        }

        [HttpGet("leave-csv")]
        public async Task<ActionResult> ExportLeaveCsv([FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate)
        {
            var report = await _reportService.GetLeaveReportAsync(fromDate, toDate);

            var sb = new StringBuilder();
            sb.AppendLine("FromDate,ToDate,TotalRequests,Pending,Approved,Rejected,Cancelled");
            sb.AppendLine($"{fromDate},{toDate},{report.TotalRequests},{report.Pending},{report.Approved},{report.Rejected},{report.Cancelled}");

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"leave_report_{fromDate}_{toDate}.csv");
        }

        [HttpGet("dashboard-json")]
        public async Task<ActionResult> ExportDashboardJson()
        {
            var dashboard = await _reportService.GetDashboardAsync();
            var json = System.Text.Json.JsonSerializer.Serialize(dashboard, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            return File(Encoding.UTF8.GetBytes(json), "application/json", $"dashboard_{DateTime.Now:yyyyMMdd}.json");
        }
    }
}
