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
        Task<OvertimeMonthlySummaryDto> GetMonthlySummaryAsync(string userIdentifier, int month, int year, bool companyWide = false);
        Task<OvertimeRequestDto> CreateAsync(string userIdentifier, AddOvertimeRequestDto dto);
        Task<OvertimeRequestDto> ApproveAsync(Guid id, string approvedBy, ReviewOvertimeRequestDto? review = null);
        Task<OvertimeRequestDto> RejectAsync(Guid id, string rejectedBy, ReviewOvertimeRequestDto? review = null);
        Task<OvertimeRequestDto> CancelAsync(Guid id, string userIdentifier);
    }
}
