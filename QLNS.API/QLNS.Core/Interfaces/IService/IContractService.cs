using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces.IService
{
    public interface IContractService
    {
        Task<IEnumerable<ContractDto>> GetAllAsync();
        Task<PagedResult<ContractDto>> GetFilterAsync(int pageIndex, int pageSize, string? search);
        Task<ContractDto> GetByIdAsync(Guid id);
        Task<IEnumerable<ContractDto>> GetByEmployeeIdAsync(Guid employeeId);
        Task<IEnumerable<ContractDto>> GetExpiringContractsAsync(int withinDays);
        Task<ContractDto> AddAsync(AddContractDto dto);
        Task<ContractDto> UpdateAsync(Guid id, AddContractDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
