using QLNS.Core.Dto;
using QLNS.Core.Interfaces;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces
{
    public interface IBonusPenaltyRepository : IGenericRepository<BonusPenalty>
    {
        Task<IReadOnlyList<BonusPenalty>> GetByEmployeeAndPeriodAsync(Guid employeeId, int month, int year);
        Task<IReadOnlyList<BonusPenalty>> GetByPeriodAsync(int month, int year);
    }
}
