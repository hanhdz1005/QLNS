using Microsoft.EntityFrameworkCore;
using QLNS.Core.Interfaces;
using QLNSCore.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QLNS.Infrastructure.Respository
{
    public class BonusPenaltyRepository : GenericRepository<BonusPenalty>, IBonusPenaltyRepository
    {
        public BonusPenaltyRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<BonusPenalty>> GetByEmployeeAndPeriodAsync(Guid employeeId, int month, int year)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(bp => bp.EmployeeId == employeeId && bp.Month == month && bp.Year == year)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<BonusPenalty>> GetByPeriodAsync(int month, int year)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(bp => bp.Month == month && bp.Year == year)
                .ToListAsync();
        }
    }
}
