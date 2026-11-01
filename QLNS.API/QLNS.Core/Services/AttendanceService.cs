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

        // B1: Đăng ký đặc điểm nhận diện khuôn mặt lần đầu
        public async Task<(bool Success, string Message, string? ErrorCode, string? ImageUrl)> RegisterFaceAsync(
            string userIdentifier,
            FaceRegisterDto dto)
        {
            if (!FaceRecognitionHelper.IsValidDescriptor(dto.FaceDescriptor))
            {
                return (false, "Không trích xuất được đặc điểm khuôn mặt hợp lệ. Vui lòng chụp rõ khuôn mặt.", "FACE_NOT_DETECTED", null);
            }

            var serializedDescriptor = FaceRecognitionHelper.SerializeDescriptor(dto.FaceDescriptor);
            var safePrefix = userIdentifier.Replace("@", "_").Replace(".", "_");
            var imageUrl = await FaceRecognitionHelper.SaveBase64ImageAsync(dto.CapturedImageBase64, "faces", safePrefix);

            var employee = await _uow.EmployeeRepository.GetByIdentifierAsync(userIdentifier);
            if (employee != null)
            {
                employee.FaceEmbedding = serializedDescriptor;
                employee.FaceImageUrl = imageUrl;
                employee.FaceRegisteredAt = DateTime.UtcNow;
                await _uow.EmployeeRepository.UpdateAsync(employee.Id, employee);
            }

            return (true, "Đăng ký nhận diện khuôn mặt thành công.", null, imageUrl);
        }

        // B3 & B4: Đối sánh khuôn mặt gửi lên với dữ liệu DB và ghi nhận chấm công
        public async Task<FaceCheckInResultDto> FaceCheckInAsync(
            string userIdentifier,
            FaceCheckInDto dto,
            string? fallbackUserEmbedding = null)
        {
            if (!FaceRecognitionHelper.IsValidDescriptor(dto.CapturedFaceDescriptor))
            {
                return new FaceCheckInResultDto
                {
                    Success = false,
                    ActionType = "Unknown",
                    ErrorCode = "FACE_NOT_DETECTED",
                    Message = "Không phát hiện được khuôn mặt hợp lệ trong khung hình chụp."
                };
            }

            var employee = await _uow.EmployeeRepository.GetByIdentifierAsync(userIdentifier);
            if (employee == null)
            {
                return new FaceCheckInResultDto
                {
                    Success = false,
                    ActionType = "Unknown",
                    ErrorCode = "EMPLOYEE_NOT_FOUND",
                    Message = "Không tìm thấy hồ sơ nhân viên tương ứng với tài khoản đăng nhập."
                };
            }

            var storedEmbeddingJson = !string.IsNullOrWhiteSpace(employee.FaceEmbedding)
                ? employee.FaceEmbedding
                : fallbackUserEmbedding;

            var storedDescriptor = FaceRecognitionHelper.DeserializeDescriptor(storedEmbeddingJson);
            if (storedDescriptor == null || !FaceRecognitionHelper.IsValidDescriptor(storedDescriptor))
            {
                return new FaceCheckInResultDto
                {
                    Success = false,
                    ActionType = "Unknown",
                    ErrorCode = "FACE_NOT_REGISTERED",
                    Message = "Tài khoản chưa đăng ký dữ liệu khuôn mặt lần đầu trong hệ thống."
                };
            }

            var (isMatch, distance, similarityPercent) = FaceRecognitionHelper.VerifyFaceMatch(
                storedDescriptor,
                dto.CapturedFaceDescriptor);

            if (!isMatch)
            {
                return new FaceCheckInResultDto
                {
                    Success = false,
                    ActionType = "Unknown",
                    ErrorCode = "FACE_MISMATCH",
                    SimilarityScore = similarityPercent,
                    Message = $"Khuôn mặt không khớp với dữ liệu nhận diện gốc (Độ tương đồng: {similarityPercent}%, Khoảng cách: {distance})."
                };
            }

            var snapshotUrl = await FaceRecognitionHelper.SaveBase64ImageAsync(
                dto.CapturedImageBase64,
                "attendances",
                employee.Code);

            return await RecordCheckInOrOutAsync(employee, snapshotUrl, similarityPercent);
        }

        private async Task<FaceCheckInResultDto> RecordCheckInOrOutAsync(
            Employee employee,
            string? snapshotUrl,
            decimal similarityPercent)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var nowTime = DateTime.Now.TimeOfDay;
            nowTime = new TimeSpan(nowTime.Hours, nowTime.Minutes, nowTime.Seconds);

            var existingAttendance = await _uow.AttendanceRepository.Query()
                .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id && a.Date == today);

            if (existingAttendance == null)
            {
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
                    Status = isLate ? AttendanceStatus.Late : AttendanceStatus.Present,
                    FaceSnapshotUrl = snapshotUrl,
                    SimilarityScore = similarityPercent
                };

                await _uow.AttendanceRepository.AddAsync(attendance);

                return new FaceCheckInResultDto
                {
                    Success = true,
                    ActionType = "CheckIn",
                    Message = isLate
                        ? $"Điểm danh khuôn mặt VÀO thành công lúc {nowTime:hh\\:mm\\:ss} (Muộn hơn 08:30 - Khớp {similarityPercent}%)."
                        : $"Điểm danh khuôn mặt VÀO thành công lúc {nowTime:hh\\:mm\\:ss} (Khớp {similarityPercent}%).",
                    EmployeeName = employee.FullName,
                    Date = today,
                    Time = nowTime,
                    HoursWorked = 0,
                    SimilarityScore = similarityPercent,
                    SnapshotUrl = snapshotUrl
                };
            }
            else
            {
                existingAttendance.CheckOut = nowTime;
                if (existingAttendance.CheckIn.HasValue)
                {
                    var worked = (decimal)(nowTime - existingAttendance.CheckIn.Value).TotalHours;
                    existingAttendance.HoursWorked = worked > 0 ? Math.Round(worked, 2) : 0;
                }
                if (!string.IsNullOrEmpty(snapshotUrl))
                {
                    existingAttendance.FaceSnapshotUrl = snapshotUrl;
                }
                existingAttendance.SimilarityScore = similarityPercent;

                await _uow.AttendanceRepository.UpdateAsync(existingAttendance.Id, existingAttendance);

                return new FaceCheckInResultDto
                {
                    Success = true,
                    ActionType = "CheckOut",
                    Message = $"Điểm danh khuôn mặt RA thành công lúc {nowTime:hh\\:mm\\:ss}. Tổng giờ: {existingAttendance.HoursWorked:F2}h (Khớp {similarityPercent}%).",
                    EmployeeName = employee.FullName,
                    Date = today,
                    Time = nowTime,
                    HoursWorked = existingAttendance.HoursWorked,
                    SimilarityScore = similarityPercent,
                    SnapshotUrl = snapshotUrl
                };
            }
        }

        // Legacy QR methods
        public async Task<(string Token, int ExpiresInSeconds)> GenerateQrTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N") + DateTime.UtcNow.Ticks.ToString("x");
            var cacheKey = $"QR_ATTENDANCE_{token}";
            _cache.Set(cacheKey, true, TimeSpan.FromSeconds(35));
            return await Task.FromResult((token, 30));
        }

        public async Task<QrCheckInResultDto> QrCheckInAsync(string qrToken, string userIdentifier)
        {
            var cacheKey = $"QR_ATTENDANCE_{qrToken}";
            if (!_cache.TryGetValue(cacheKey, out _))
            {
                return new QrCheckInResultDto
                {
                    Success = false,
                    ActionType = "Unknown",
                    Message = "Mã QR đã hết hạn hoặc không hợp lệ."
                };
            }

            var employee = await _uow.EmployeeRepository.GetByIdentifierAsync(userIdentifier);
            if (employee == null)
            {
                return new QrCheckInResultDto
                {
                    Success = false,
                    ActionType = "Unknown",
                    Message = "Không tìm thấy hồ sơ nhân viên tương ứng."
                };
            }

            var res = await RecordCheckInOrOutAsync(employee, null, 100m);
            return new QrCheckInResultDto
            {
                Success = res.Success,
                ActionType = res.ActionType,
                Message = res.Message,
                EmployeeName = res.EmployeeName,
                Date = res.Date,
                Time = res.Time,
                HoursWorked = res.HoursWorked
            };
        }

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
