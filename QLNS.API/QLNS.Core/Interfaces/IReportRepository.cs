using QLNS.Core.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces
{
    public interface IReportRepository
    {
        Task<DashboardDto> GetDashboardAsync();
        Task<IReadOnlyList<EmployeeStatsDto>> GetEmployeeStatsByDepartmentAsync();
        Task<LeaveReportDto> GetLeaveReportAsync(DateOnly fromDate, DateOnly toDate);
        Task<SalaryReportDto> GetSalaryReportAsync(int month, int year);
        Task<HrAlertsDto> GetHrAlertsAsync();
    }
}
