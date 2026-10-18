using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces.IService
{
    public interface IBonusPenaltyService
    {
        Task<IEnumerable<BonusPenaltyDto>> GetAllAsync();
        Task<PagedResult<BonusPenaltyDto>> GetFilterAsync(int pageIndex, int pageSize, string? search, int? type);
        Task<BonusPenaltyDto> GetByIdAsync(Guid id);
        Task<BonusPenaltyDto> AddAsync(AddBonusPenaltyDto dto);
        Task<BonusPenaltyDto> UpdateAsync(Guid id, AddBonusPenaltyDto dto);
        Task<bool> DeleteAsync(Guid id);
    }

    public interface IPayrollService
    {
        Task<IEnumerable<PayrollDto>> GetAllAsync(int month, int year);
        Task<PayrollDto> GeneratePayrollAsync(Guid employeeId, int month, int year, bool saveToDb = true);
        Task<IEnumerable<PayrollDto>> GenerateBulkPayrollAsync(int month, int year);
        Task<IEnumerable<PayrollDto>> GenerateBulkPayrollAsync(PayrollGenerateDto dto);
        Task<PayrollDto> GetMyPayslipAsync(string userIdentifier, int month, int year);
    }

    public interface IReportService
    {
        Task<DashboardDto> GetDashboardAsync();
        Task<IReadOnlyList<EmployeeStatsDto>> GetEmployeeStatsByDepartmentAsync();
        Task<LeaveReportDto> GetLeaveReportAsync(DateOnly fromDate, DateOnly toDate);
        Task<SalaryReportDto> GetSalaryReportAsync(int month, int year);
        Task<HrAlertsDto> GetHrAlertsAsync();
    }

    public interface ILeaveApprovalService
    {
        Task<LeaveRequestDto> ApproveAsync(Guid id, string approvedBy);
        Task<LeaveRequestDto> RejectAsync(Guid id, string rejectedBy);
        Task<PagedResult<LeaveRequestDto>> GetPendingApprovalsAsync(int pageIndex, int pageSize);
    }
}
