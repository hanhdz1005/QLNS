using QLNS.Core.Dto;
using System;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces.IService
{
    public interface ILeaveBalanceService
    {
        Task<LeaveBalanceDto> GetByEmployeeAndYearAsync(Guid employeeId, int year);
        Task<LeaveBalanceDto> GetMyBalanceAsync(string userIdentifier, int year);
        Task<LeaveBalanceDto> UpdateTotalDaysAsync(Guid employeeId, int year, decimal totalDays);
        Task DeductLeaveDaysAsync(Guid employeeId, int year, decimal days);
    }
}
