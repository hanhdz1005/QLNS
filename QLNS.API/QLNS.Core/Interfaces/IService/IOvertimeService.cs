using QLNS.Core.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces.IService
{
    public interface IOvertimeService
    {
        Task<IEnumerable<OvertimeRequestDto>> GetMyRequestsAsync(string userIdentifier);
        Task<IEnumerable<OvertimeRequestDto>> GetAllRequestsAsync();
        Task<IEnumerable<OvertimeRequestDto>> GetPendingApprovalsAsync();
        Task<OvertimeRequestDto> CreateAsync(string userIdentifier, AddOvertimeRequestDto dto);
        Task<OvertimeRequestDto> ApproveAsync(Guid id, string approvedBy);
        Task<OvertimeRequestDto> RejectAsync(Guid id, string rejectedBy);
    }
}
