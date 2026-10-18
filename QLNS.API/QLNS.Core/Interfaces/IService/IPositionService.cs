using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces.IService
{
    public interface IPositionService
    {
        Task<IEnumerable<PositionDto>> GetAllAsync();
        Task<PagedResult<PositionDto>> GetFilterAsync(int pageIndex, int pageSize, string? search);
        Task<PositionDto> GetByIdAsync(Guid id);
        Task<PositionDto> AddAsync(AddPositionDto dto);
        Task<PositionDto> UpdateAsync(Guid id, AddPositionDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
