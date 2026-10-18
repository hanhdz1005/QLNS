using System;

namespace QLNS.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IAttendanceRepository AttendanceRepository { get; }
        IDepartmentRepository DepartmentRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        ILeaveRequestRepository LeaveRequestRepository { get; }
        IPositionRepository PositionRepository { get; }
        ISalaryRepository SalaryRepository { get; }
        IBonusPenaltyRepository BonusPenaltyRepository { get; }
        IReportRepository ReportRepository { get; }
        IContractRepository ContractRepository { get; }
        ILeaveBalanceRepository LeaveBalanceRepository { get; }
    }
}
