using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces.IService
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync();
        Task<PagedResult<EmployeeDto>> GetFilterAsync(int pageIndex, int pageSize, string? search);
        Task<EmployeeDto> GetByIdAsync(Guid id);
        Task<EmployeeDto> AddAsync(AddEmployeeDto dto);
        Task<EmployeeDto> UpdateAsync(Guid id, AddEmployeeDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<EmployeeDto> GetProfileAsync(string userIdentifier);
    }
}
