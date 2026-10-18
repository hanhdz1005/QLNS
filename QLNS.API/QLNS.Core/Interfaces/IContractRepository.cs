using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces
{
    public interface IContractRepository : IGenericRepository<Contract>
    {
        Task<IEnumerable<Contract>> GetByEmployeeIdAsync(Guid employeeId);
        Task<IEnumerable<Contract>> GetExpiringContractsAsync(int withinDays);
    }
}
