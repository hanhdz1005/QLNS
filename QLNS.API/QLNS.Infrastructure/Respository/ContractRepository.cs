using Microsoft.EntityFrameworkCore;
using QLNS.Core.Interfaces;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLNS.Infrastructure.Respository
{
    public class ContractRepository : GenericRepository<Contract>, IContractRepository
    {
        public ContractRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Contract>> GetByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.Contract
                .Where(c => c.EmployeeId == employeeId)
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetExpiringContractsAsync(int withinDays)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var maxDate = today.AddDays(withinDays);

            return await _context.Contract
                .Where(c => c.Status == ContractStatus.Active &&
                            c.EndDate.HasValue &&
                            c.EndDate.Value >= today &&
                            c.EndDate.Value <= maxDate)
                .OrderBy(c => c.EndDate)
                .ToListAsync();
        }
    }
}
