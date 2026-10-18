using Microsoft.EntityFrameworkCore;
using QLNS.Core.Interfaces;
using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QLNS.Infrastructure.Respository
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var now = DateTime.UtcNow;

            var totalEmployees = await _context.Employee.CountAsync();
            var activeEmployees = await _context.Employee.CountAsync(e => e.Status == EmployeeStatus.Active);
            var totalDepartments = await _context.Department.CountAsync();
            var pendingLeaveRequests = await _context.LeaveRequest.CountAsync(l => l.Status == LeaveStatus.Pending);
            var attendancesThisMonth = await _context.Attendance
                .CountAsync(a => a.Date.Month == now.Month && a.Date.Year == now.Year);
            //var salaryStats = await _context.Salary
            //    .Where(s => s.Month == now.Month && s.Year == now.Year)
            //    .GroupBy(x => 1)
            //    .Select(g => new
            //    {
            //        TotalSalary = g.Sum(x => x.TotalSalary),
            //        AverageSalary = g.Average(x => x.TotalSalary)
            //    })
            //    .FirstOrDefaultAsync();
            var previousMonth = now.AddMonths(-1);

            var salaryStats = await _context.Salary
                .Where(s => s.Month == previousMonth.Month &&
                            s.Year == previousMonth.Year)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    TotalSalary = g.Sum(x => x.TotalSalary),
                    AverageSalary = g.Average(x => x.TotalSalary)
                })
                .FirstOrDefaultAsync();

            var salariesLastMonth = salaryStats?.TotalSalary ?? 0;
            var averageSalary = salaryStats?.AverageSalary ?? 0;
            //var salariesThisMonth = await _context.Salary
            //    .Where(s => s.Month == now.Month && s.Year == now.Year)
            //    .SumAsync(s => s.TotalSalary);
            //var averageSalary = await _context.Salary
            //    .Where(s => s.Month == now.Month && s.Year == now.Year)
            //    .DefaultIfEmpty()
            //    .AverageAsync(s => s != null ? s.TotalSalary : 0);

            return new DashboardDto
            {
                TotalEmployees = totalEmployees,
                ActiveEmployees = activeEmployees,
                TotalDepartments = totalDepartments,
                PendingLeaveRequests = pendingLeaveRequests,
                TotalAttendancesThisMonth = attendancesThisMonth,
                TotalSalaryLastMonth = salariesLastMonth,
                AverageSalary = averageSalary
            };
        }

        public async Task<IReadOnlyList<EmployeeStatsDto>> GetEmployeeStatsByDepartmentAsync()
        {
            var result = await _context.Employee
                .AsNoTracking()
                .GroupBy(e => new { e.DepartmentId, e.DepartmentName })
                .Select(g => new EmployeeStatsDto
                {
                    DepartmentId = g.Key.DepartmentId,
                    DepartmentName = g.Key.DepartmentName,
                    EmployeeCount = g.Count()
                })
                .ToListAsync();
            return result;
        }

        public async Task<LeaveReportDto> GetLeaveReportAsync(DateOnly fromDate, DateOnly toDate)
        {
            var query = _context.LeaveRequest
                .AsNoTracking()
                .Where(l => l.CreatedDate >= fromDate && l.CreatedDate <= toDate);

            var total = await query.CountAsync();

            return new LeaveReportDto
            {
                TotalRequests = total,
                Pending = await query.CountAsync(l => l.Status == LeaveStatus.Pending),
                Approved = await query.CountAsync(l => l.Status == LeaveStatus.Approved),
                Rejected = await query.CountAsync(l => l.Status == LeaveStatus.Rejected),
                Cancelled = await query.CountAsync(l => l.Status == LeaveStatus.Cancelled)
            };
        }

        public async Task<SalaryReportDto> GetSalaryReportAsync(int month, int year)
        {
            var query = _context.Salary
                .AsNoTracking()
                .Where(s => s.Month == month && s.Year == year);

            var total = await query.CountAsync();
            if (total == 0)
            {
                return new SalaryReportDto
                {
                    TotalRecords = 0,
                    TotalBasicSalary = 0,
                    TotalNetSalary = 0,
                    AverageNetSalary = 0,
                    MaxSalary = 0,
                    MinSalary = 0
                };
            }

            var salaries = await query.Select(s => new { s.BasicSalary, s.TotalSalary }).ToListAsync();

            return new SalaryReportDto
            {
                TotalRecords = total,
                TotalBasicSalary = salaries.Sum(s => s.BasicSalary),
                TotalNetSalary = salaries.Sum(s => s.TotalSalary),
                AverageNetSalary = salaries.Average(s => s.TotalSalary),
                MaxSalary = salaries.Max(s => s.TotalSalary),
                MinSalary = salaries.Min(s => s.TotalSalary)
            };
        }

        public async Task<HrAlertsDto> GetHrAlertsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var thirtyDaysLater = today.AddDays(30);

            var expiringContracts = await _context.Contract
                .AsNoTracking()
                .Where(c => c.Status == ContractStatus.Active &&
                            c.EndDate.HasValue &&
                            c.EndDate.Value >= today &&
                            c.EndDate.Value <= thirtyDaysLater)
                .OrderBy(c => c.EndDate)
                .Select(c => new ExpiringContractItemDto
                {
                    ContractId = c.Id,
                    EmployeeId = c.EmployeeId,
                    EmployeeName = c.EmployeeName,
                    ContractNumber = c.ContractNumber,
                    EndDate = c.EndDate,
                    DaysRemaining = c.EndDate.HasValue ? c.EndDate.Value.DayNumber - today.DayNumber : 0
                })
                .ToListAsync();

            var currentMonth = today.Month;
            var employees = await _context.Employee
                .AsNoTracking()
                .Where(e => e.Status == EmployeeStatus.Active)
                .ToListAsync();

            var upcomingBirthdays = employees
                .Where(e => e.DateOfBirth.Month == currentMonth)
                .OrderBy(e => e.DateOfBirth.Day)
                .Select(e => new BirthdayItemDto
                {
                    EmployeeId = e.Id,
                    EmployeeName = e.FullName,
                    DepartmentName = e.DepartmentName ?? "",
                    DateOfBirth = e.DateOfBirth,
                    Day = e.DateOfBirth.Day
                })
                .ToList();

            var probationEnding = employees
                .Select(e => new
                {
                    Employee = e,
                    ProbationEnd = e.JoinDate.AddMonths(2)
                })
                .Where(x => x.ProbationEnd >= today && x.ProbationEnd <= thirtyDaysLater)
                .OrderBy(x => x.ProbationEnd)
                .Select(x => new ProbationEndingItemDto
                {
                    EmployeeId = x.Employee.Id,
                    EmployeeName = x.Employee.FullName,
                    DepartmentName = x.Employee.DepartmentName ?? "",
                    JoinDate = x.Employee.JoinDate,
                    EstimatedProbationEnd = x.ProbationEnd,
                    DaysRemaining = x.ProbationEnd.DayNumber - today.DayNumber
                })
                .ToList();

            return new HrAlertsDto
            {
                ExpiringContracts = expiringContracts,
                UpcomingBirthdays = upcomingBirthdays,
                ProbationEndingEmployees = probationEnding
            };
        }
    }
}
