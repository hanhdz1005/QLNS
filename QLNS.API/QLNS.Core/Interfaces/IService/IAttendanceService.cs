using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces.IService
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceDto>> GetAllAsync();
        Task<PagedResult<AttendanceDto>> GetFilterAsync(int pageIndex, int pageSize, string? search);
        Task<AttendanceDto> GetByIdAsync(Guid id);
        Task<AttendanceDto> AddAsync(AddAttendanceDto dto);
        Task<AttendanceDto> UpdateAsync(Guid id, AddAttendanceDto dto);
        Task<bool> DeleteAsync(Guid id);

        // Chấm công bằng mã QR động
        Task<(string Token, int ExpiresInSeconds)> GenerateQrTokenAsync();
        Task<QrCheckInResultDto> QrCheckInAsync(string qrToken, string userIdentifier);

        // Xem chấm công cá nhân (ESS)
        Task<IEnumerable<AttendanceDto>> GetMyAttendanceAsync(string userIdentifier, int month, int year);
    }
}
