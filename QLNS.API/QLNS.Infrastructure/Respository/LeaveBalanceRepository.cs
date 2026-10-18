using Microsoft.EntityFrameworkCore;
using QLNS.Core.Interfaces;
using QLNSCore.Entities;
using System;
using System.Threading.Tasks;

namespace QLNS.Infrastructure.Respository
{
    public class LeaveBalanceRepository : GenericRepository<LeaveBalance>, ILeaveBalanceRepository
    {
        public LeaveBalanceRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<LeaveBalance?> GetByEmployeeAndYearAsync(Guid employeeId, int year)
        {
            return await _context.LeaveBalance
                .FirstOrDefaultAsync(lb => lb.EmployeeId == employeeId && lb.Year == year);
        }
    }
}
