using QLNS.Core.Dto;
using QLNS.Core.Interfaces;
using QLNS.Core.Interfaces.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNS.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public Task<DashboardDto> GetDashboardAsync()
        {
            return _reportRepository.GetDashboardAsync();
        }

        public Task<IReadOnlyList<EmployeeStatsDto>> GetEmployeeStatsByDepartmentAsync()
        {
            return _reportRepository.GetEmployeeStatsByDepartmentAsync();
        }

        public Task<LeaveReportDto> GetLeaveReportAsync(DateOnly fromDate, DateOnly toDate)
        {
            return _reportRepository.GetLeaveReportAsync(fromDate, toDate);
        }

        public Task<SalaryReportDto> GetSalaryReportAsync(int month, int year)
        {
            return _reportRepository.GetSalaryReportAsync(month, year);
        }

        public Task<HrAlertsDto> GetHrAlertsAsync()
        {
            return _reportRepository.GetHrAlertsAsync();
        }
    }
}
