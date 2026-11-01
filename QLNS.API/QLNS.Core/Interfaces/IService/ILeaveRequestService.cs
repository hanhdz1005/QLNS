using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces.IService
{
    public interface ILeaveRequestService
    {
        Task<IEnumerable<LeaveRequestDto>> GetAllAsync();
        Task<PagedResult<LeaveRequestDto>> GetFilterAsync(int pageIndex, int pageSize, string? search, string? userIdentifier = null);
        Task<LeaveRequestDto> GetByIdAsync(Guid id);
        Task<LeaveRequestDto> AddAsync(AddLeaveRequestDto dto);
        Task<LeaveRequestDto> UpdateAsync(Guid id, AddLeaveRequestDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
