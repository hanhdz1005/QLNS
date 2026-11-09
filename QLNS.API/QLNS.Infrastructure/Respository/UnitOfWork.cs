using QLNS.Core.Interfaces;

namespace QLNS.Infrastructure.Respository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IAttendanceRepository AttendanceRepository { get; set; }
        public IEmployeeRepository EmployeeRepository { get; set; }
        public IDepartmentRepository DepartmentRepository { get; set; }
        public ILeaveRequestRepository LeaveRequestRepository { get; set; }
        public IPositionRepository PositionRepository { get; set; }
        public ISalaryRepository SalaryRepository { get; set; }
        public IBonusPenaltyRepository BonusPenaltyRepository { get; }
        public IReportRepository ReportRepository { get; }
        public IContractRepository ContractRepository { get; }
        public ILeaveBalanceRepository LeaveBalanceRepository { get; }
        public IOvertimeRequestRepository OvertimeRequestRepository { get; }

        public UnitOfWork(AppDbContext context)
        {
            AttendanceRepository = new AttendanceRepository(context);
            EmployeeRepository = new EmployeeRepository(context);
            DepartmentRepository = new DepartmentRepository(context);
            LeaveRequestRepository = new LeaveRequestRepository(context);
            PositionRepository = new PositionRepository(context);
            SalaryRepository = new SalaryRepository(context);
            BonusPenaltyRepository = new BonusPenaltyRepository(context);
            ReportRepository = new ReportRepository(context);
            ContractRepository = new ContractRepository(context);
            LeaveBalanceRepository = new LeaveBalanceRepository(context);
            OvertimeRequestRepository = new OvertimeRequestRepository(context);
        }
    }
}
