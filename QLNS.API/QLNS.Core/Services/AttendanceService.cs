using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QLNS.Core.Common;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces.IService;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLNS.Core.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public AttendanceService(IUnitOfWork uow, IMapper mapper, IMemoryCache cache)
        {
            _uow = uow;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<AttendanceDto>> GetAllAsync()
        {
            var items = await _uow.AttendanceRepository
                .Query()
                .Include(a => a.Employee)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
            return _mapper.Map<IReadOnlyList<AttendanceDto>>(items);
        }

        public async Task<PagedResult<AttendanceDto>> GetFilterAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _uow.AttendanceRepository
                .Query()
                .Include(a => a.Employee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(a => a.EmployeeName.ToLower().Contains(search));
            }

            return await query
                .OrderByDescending(a => a.Date)
                .ToPagedResultAsync<Attendance, AttendanceDto>(pageIndex, pageSize, _mapper);
        }

        public async Task<AttendanceDto> GetByIdAsync(Guid id)
        {
            var item = await _uow.AttendanceRepository
                .Query()
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (item == null)
                throw new KeyNotFoundException("Not found attendance");
            return _mapper.Map<AttendanceDto>(item);
        }

        public async Task<AttendanceDto> AddAsync(AddAttendanceDto dto)
        {
            var employee = await _uow.EmployeeRepository.GetAsync(dto.EmployeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var entity = _mapper.Map<Attendance>(dto);
            entity.EmployeeName = employee.FullName;
            if (entity.CheckIn.HasValue && entity.CheckOut.HasValue)
                entity.HoursWorked = (decimal)(entity.CheckOut.Value - entity.CheckIn.Value).TotalHours;
            await _uow.AttendanceRepository.AddAsync(entity);
            return _mapper.Map<AttendanceDto>(entity);
        }

        public async Task<AttendanceDto> UpdateAsync(Guid id, AddAttendanceDto dto)
        {
            var exist = await _uow.AttendanceRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found attendance");

            var employee = await _uow.EmployeeRepository.GetAsync(dto.EmployeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var entity = _mapper.Map<Attendance>(dto);
            entity.EmployeeName = employee.FullName;
            if (entity.CheckIn.HasValue && entity.CheckOut.HasValue)
                entity.HoursWorked = (decimal)(entity.CheckOut.Value - entity.CheckIn.Value).TotalHours;
            entity.Id = id;
            await _uow.AttendanceRepository.UpdateAsync(id, entity);
            return _mapper.Map<AttendanceDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var exist = await _uow.AttendanceRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found attendance");
            await _uow.AttendanceRepository.DeleteAsync(id);
            return true;
        }

        // 1. Sinh mã QR động (reset sau 30s)
        public async Task<(string Token, int ExpiresInSeconds)> GenerateQrTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N") + DateTime.UtcNow.Ticks.ToString("x");
            var cacheKey = $"QR_ATTENDANCE_{token}";
            // Lưu cache với TTL 35 giây (30s hiển thị + 5s buffer độ trễ mạng)
            _cache.Set(cacheKey, true, TimeSpan.FromSeconds(35));
            return await Task.FromResult((token, 30));
        }

        // 2. Điểm danh qua quét mã QR
        public async Task<QrCheckInResultDto> QrCheckInAsync(string qrToken, string userIdentifier)
        {
            var cacheKey = $"QR_ATTENDANCE_{qrToken}";
            if (!_cache.TryGetValue(cacheKey, out _))
            {
                return new QrCheckInResultDto
                {
                    Success = false,
                    ActionType = "Unknown",
                    Message = "Mã QR đã hết hạn hoặc không hợp lệ. Vui lòng quét lại mã QR mới nhất hiển thị trên màn hình."
                };
            }

            var employee = await _uow.EmployeeRepository.GetByIdentifierAsync(userIdentifier);

            if (employee == null)
            {
                return new QrCheckInResultDto
                {
                    Success = false,
                    ActionType = "Unknown",
                    Message = "Không tìm thấy hồ sơ nhân viên tương ứng với tài khoản đăng nhập."
                };
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var nowTime = DateTime.Now.TimeOfDay;
            nowTime = new TimeSpan(nowTime.Hours, nowTime.Minutes, nowTime.Seconds);

            var existingAttendance = await _uow.AttendanceRepository.Query()
                .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id && a.Date == today);

            if (existingAttendance == null)
            {
                // Điểm danh vào ca làm việc đầu tiên trong ngày
                var isLate = nowTime > new TimeSpan(8, 30, 0);
                var attendance = new Attendance
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    EmployeeName = employee.FullName,
                    Date = today,
                    CheckIn = nowTime,
                    CheckOut = null,
                    HoursWorked = 0,
                    Status = isLate ? AttendanceStatus.Late : AttendanceStatus.Present
                };

                await _uow.AttendanceRepository.AddAsync(attendance);

                return new QrCheckInResultDto
                {
                    Success = true,
                    ActionType = "CheckIn",
                    Message = isLate 
                        ? $"Điểm danh VÀO thành công lúc {nowTime:hh\\:mm\\:ss} (Muộn hơn quy định 08:30)." 
                        : $"Điểm danh VÀO thành công lúc {nowTime:hh\\:mm\\:ss}.",
                    EmployeeName = employee.FullName,
                    Date = today,
                    Time = nowTime,
                    HoursWorked = 0
                };
            }
            else
            {
                // Điểm danh ra về (hoặc cập nhật lần ra mới nhất)
                existingAttendance.CheckOut = nowTime;
                if (existingAttendance.CheckIn.HasValue)
                {
                    var worked = (decimal)(nowTime - existingAttendance.CheckIn.Value).TotalHours;
                    existingAttendance.HoursWorked = worked > 0 ? Math.Round(worked, 2) : 0;
                }

                await _uow.AttendanceRepository.UpdateAsync(existingAttendance.Id, existingAttendance);

                return new QrCheckInResultDto
                {
                    Success = true,
                    ActionType = "CheckOut",
                    Message = $"Điểm danh RA thành công lúc {nowTime:hh\\:mm\\:ss}. Tổng thời gian làm việc: {existingAttendance.HoursWorked:F2} giờ.",
                    EmployeeName = employee.FullName,
                    Date = today,
                    Time = nowTime,
                    HoursWorked = existingAttendance.HoursWorked
                };
            }
        }

        // 3. Xem danh sách chấm công của nhân viên đang đăng nhập (Employee Self-Service)
        public async Task<IEnumerable<AttendanceDto>> GetMyAttendanceAsync(string userIdentifier, int month, int year)
        {
            var employee = await _uow.EmployeeRepository.GetByIdentifierAsync(userIdentifier);

            if (employee == null)
                throw new KeyNotFoundException("Không tìm thấy nhân viên.");

            var list = await _uow.AttendanceRepository.Query()
                .Where(a => a.EmployeeId == employee.Id && a.Date.Month == month && a.Date.Year == year)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return _mapper.Map<IReadOnlyList<AttendanceDto>>(list);
        }
    }
}