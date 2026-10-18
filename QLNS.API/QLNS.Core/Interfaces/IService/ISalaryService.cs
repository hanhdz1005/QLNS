using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces.IService
{
    public interface ISalaryService
    {
        Task<IEnumerable<SalariesDto>> GetAllAsync();
        Task<PagedResult<SalariesDto>> GetFilterAsync(int pageIndex, int pageSize, string? search);
        Task<SalariesDto> GetByIdAsync(Guid id);
        Task<SalariesDto> AddAsync(AddSalariesDto dto);
        Task<SalariesDto> UpdateAsync(Guid id, AddSalariesDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
