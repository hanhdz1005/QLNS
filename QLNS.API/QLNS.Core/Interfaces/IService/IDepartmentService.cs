using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces.IService
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();
        Task<PagedResult<DepartmentDto>> GetFilterAsync(int pageIndex, int pageSize, string? search);
        Task<DepartmentDto> GetByIdAsync(Guid id);
        Task<DepartmentDto> AddAsync(AddDepartmentDto department);
        Task<DepartmentDto> UpdateAsync(Guid id, AddDepartmentDto department);
        Task<bool> DeleteAsync(Guid id);
    }
}
