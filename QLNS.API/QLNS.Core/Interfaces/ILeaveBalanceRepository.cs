using QLNSCore.Entities;
using System;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces
{
    public interface ILeaveBalanceRepository : IGenericRepository<LeaveBalance>
    {
        Task<LeaveBalance?> GetByEmployeeAndYearAsync(Guid employeeId, int year);
    }
}
