using Microsoft.EntityFrameworkCore;
using QLNS.Core.Common;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces;
using QLNS.Core.Interfaces.IService;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLNS.Core.Services
{
    public class OvertimeService : IOvertimeService
    {
        private readonly IUnitOfWork _uow;

        public OvertimeService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        private static decimal ResolveMultiplier(OvertimeType otType) => otType switch
        {
            OvertimeType.Weekend => 2.00m,
            OvertimeType.Holiday => 3.00m,
            _ => 1.50m
        };

        private static OvertimeType ResolveDefaultOtType(DateOnly date)
        {
            var dow = date.DayOfWeek;
            return dow == DayOfWeek.Sunday ? OvertimeType.Weekend : OvertimeType.Weekday;
        }

        private static OvertimeRequestDto MapToDto(OvertimeRequest entity, Attendance? att = null)
        {
            return new OvertimeRequestDto
            {
                Id = entity.Id,
                EmployeeId = entity.EmployeeId,
                EmployeeCode = entity.Employee?.Code ?? string.Empty,
                EmployeeName = entity.EmployeeName,
                DepartmentName = entity.Employee?.DepartmentName ?? string.Empty,
                Date = entity.Date,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                RegisteredHours = entity.RegisteredHours,
                OtType = entity.OtType != 0 ? entity.OtType : ResolveDefaultOtType(entity.Date),
                Multiplier = entity.Multiplier > 0 ? entity.Multiplier : ResolveMultiplier(entity.OtType),
                CompensationType = entity.CompensationType != 0 ? entity.CompensationType : OvertimeCompensationType.CompensatoryLeave,
                Reason = entity.Reason,
                Status = entity.Status,
                ApprovedBy = entity.ApprovedBy,
                ApproverNote = entity.ApproverNote,
                CreatedDate = entity.CreatedDate,
                ActualCheckIn = att?.CheckIn,
                ActualCheckOut = att?.CheckOut,
                ActualHoursWorked = att?.HoursWorked
            };
        }

        private async Task<List<OvertimeRequestDto>> MapListWithAttendanceAsync(List<OvertimeRequest> items)
        {
            if (items.Count == 0)
                return new List<OvertimeRequestDto>();

            var empIds = items.Select(x => x.EmployeeId).Distinct().ToList();
            var minDate = items.Min(x => x.Date);
            var maxDate = items.Max(x => x.Date);

            var attendances = await _uow.AttendanceRepository
                .Query()
                .Where(a => empIds.Contains(a.EmployeeId) && a.Date >= minDate && a.Date <= maxDate)
                .ToListAsync();

            var attLookup = attendances
                .GroupBy(a => (a.EmployeeId, a.Date))
                .ToDictionary(g => g.Key, g => g.First());

            return items.Select(item =>
            {
                attLookup.TryGetValue((item.EmployeeId, item.Date), out var att);
                return MapToDto(item, att);
            }).ToList();
        }

        private async Task SyncAttendanceOvertimeAsync(Guid employeeId, DateOnly date)
        {
            var attendance = await _uow.AttendanceRepository
                .Query()
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == date);

            if (attendance == null)
                return;

            var approvedOtHours = await _uow.OvertimeRequestRepository
                .Query()
                .Where(o => o.EmployeeId == employeeId && o.Date == date && o.Status == OvertimeStatus.Approved)
                .SumAsync(o => (decimal?)o.RegisteredHours) ?? 0m;

            var (regHours, otHours, totalHours) = PayrollCalculator.CalculateDailyWorkingHours(
                attendance.CheckIn,
                attendance.CheckOut,
                approvedOtHours);

            attendance.RegularHours = regHours;
            attendance.OvertimeHours = otHours;
            attendance.HoursWorked = totalHours;

            await _uow.AttendanceRepository.UpdateAsync(attendance.Id, attendance);
        }

        private async Task CreditCompensatoryLeaveAsync(Employee employee, int year, decimal hoursDiff)
        {
            if (Math.Abs(hoursDiff) < 0.01m)
                return;

            var daysDelta = Math.Round(hoursDiff / 8.0m, 2);
            var balance = await _uow.LeaveBalanceRepository
                .Query()
                .FirstOrDefaultAsync(b => b.EmployeeId == employee.Id && b.Year == year);

            if (balance == null)
            {
                if (daysDelta <= 0) return;
                var newBalance = new LeaveBalance
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    EmployeeName = employee.FullName,
                    Year = year,
                    TotalLeaveDays = Math.Round(12m + daysDelta, 2),
                    UsedLeaveDays = 0m,
                    RemainingLeaveDays = Math.Round(12m + daysDelta, 2)
                };
                await _uow.LeaveBalanceRepository.AddAsync(newBalance);
            }
            else
            {
                balance.TotalLeaveDays = Math.Max(0m, Math.Round(balance.TotalLeaveDays + daysDelta, 2));
                balance.RemainingLeaveDays = Math.Max(0m, Math.Round(balance.TotalLeaveDays - balance.UsedLeaveDays, 2));
                await _uow.LeaveBalanceRepository.UpdateAsync(balance.Id, balance);
            }
        }

        public async Task<IEnumerable<OvertimeRequestDto>> GetMyRequestsAsync(string userIdentifier)
        {
            var employee = await _uow.EmployeeRepository.GetByIdentifierAsync(userIdentifier);
            if (employee == null)
                return Enumerable.Empty<OvertimeRequestDto>();

            var items = await _uow.OvertimeRequestRepository
                .Query()
                .Include(o => o.Employee)
                .Where(o => o.EmployeeId == employee.Id)
                .OrderByDescending(o => o.Date)
                .ThenByDescending(o => o.CreatedDate)
                .ToListAsync();

            return await MapListWithAttendanceAsync(items);
        }

        public async Task<IEnumerable<OvertimeRequestDto>> GetAllRequestsAsync()
        {
            var items = await _uow.OvertimeRequestRepository
                .Query()
                .Include(o => o.Employee)
                .OrderByDescending(o => o.Date)
                .ThenByDescending(o => o.CreatedDate)
                .ToListAsync();

            return await MapListWithAttendanceAsync(items);
        }

        public async Task<IEnumerable<OvertimeRequestDto>> GetPendingApprovalsAsync()
        {
            var items = await _uow.OvertimeRequestRepository
                .Query()
                .Include(o => o.Employee)
                .Where(o => o.Status == OvertimeStatus.Pending)
                .OrderByDescending(o => o.Date)
                .ToListAsync();

            return await MapListWithAttendanceAsync(items);
        }

        public async Task<OvertimeMonthlySummaryDto> GetMonthlySummaryAsync(
            string userIdentifier,
            int month,
            int year,
            bool companyWide = false)
        {
            var query = _uow.OvertimeRequestRepository
                .Query()
                .Where(o => o.Date.Month == month && o.Date.Year == year);

            if (!companyWide)
            {
                var employee = await _uow.EmployeeRepository.GetByIdentifierAsync(userIdentifier);
                if (employee == null)
                    return new OvertimeMonthlySummaryDto { Month = month, Year = year };

                query = query.Where(o => o.EmployeeId == employee.Id);
            }

            var items = await query.ToListAsync();
            var approved = items.Where(o => o.Status == OvertimeStatus.Approved).ToList();

            var compHours = approved
                .Where(o => o.CompensationType == OvertimeCompensationType.CompensatoryLeave || o.CompensationType == 0)
                .Sum(o => o.RegisteredHours);
            var recHours = approved
                .Where(o => o.CompensationType == OvertimeCompensationType.RecognitionOnly)
                .Sum(o => o.RegisteredHours);
            var paidHours = approved
                .Where(o => o.CompensationType == OvertimeCompensationType.Paid)
                .Sum(o => o.RegisteredHours);

            return new OvertimeMonthlySummaryDto
            {
                Month = month,
                Year = year,
                TotalApprovedHours = Math.Round(approved.Sum(o => o.RegisteredHours), 2),
                MonthlyLimitHours = 40.0m,
                CompensatoryHours = Math.Round(compHours, 2),
                CompensatoryDaysAdded = Math.Round(compHours / 8.0m, 2),
                RecognitionHours = Math.Round(recHours, 2),
                PaidHours = Math.Round(paidHours, 2),
                PendingCount = items.Count(o => o.Status == OvertimeStatus.Pending)
            };
        }

        public async Task<OvertimeRequestDto> CreateAsync(string userIdentifier, AddOvertimeRequestDto dto)
        {
            Employee? employee = null;
            if (dto.EmployeeId.HasValue && dto.EmployeeId.Value != Guid.Empty)
            {
                employee = await _uow.EmployeeRepository.GetAsync(dto.EmployeeId.Value);
            }
            if (employee == null)
            {
                employee = await _uow.EmployeeRepository.GetByIdentifierAsync(userIdentifier);
            }
            if (employee == null)
            {
                throw new KeyNotFoundException("Không tìm thấy hồ sơ nhân viên để đăng ký OT.");
            }

            var hours = dto.RegisteredHours;
            if (dto.StartTime.HasValue && dto.EndTime.HasValue)
            {
                if (dto.EndTime.Value <= dto.StartTime.Value)
                    throw new InvalidOperationException("Giờ kết thúc OT phải lớn hơn giờ bắt đầu.");

                var calcHours = (decimal)(dto.EndTime.Value - dto.StartTime.Value).TotalHours;
                if (hours <= 0)
                    hours = Math.Round(calcHours, 2);
            }

            if (hours <= 0 || hours > 16)
            {
                throw new InvalidOperationException("Số giờ đăng ký OT phải từ 0.5 đến 16 giờ.");
            }

            // Kiểm tra trùng đơn OT đang hoạt động trong cùng ngày
            var existingOnDate = await _uow.OvertimeRequestRepository
                .Query()
                .Where(o => o.EmployeeId == employee.Id
                         && o.Date == dto.Date
                         && (o.Status == OvertimeStatus.Pending || o.Status == OvertimeStatus.Approved))
                .ToListAsync();

            if (existingOnDate.Count > 0 && dto.StartTime.HasValue && dto.EndTime.HasValue)
            {
                var overlapped = existingOnDate.Any(o =>
                    o.StartTime.HasValue && o.EndTime.HasValue &&
                    dto.StartTime.Value < o.EndTime.Value && dto.EndTime.Value > o.StartTime.Value);

                if (overlapped)
                    throw new InvalidOperationException("Khung giờ đăng ký OT bị trùng với một đơn OT khác trong cùng ngày.");
            }

            var otType = dto.OtType ?? ResolveDefaultOtType(dto.Date);
            var compType = dto.CompensationType ?? OvertimeCompensationType.CompensatoryLeave;

            var entity = new OvertimeRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                EmployeeName = employee.FullName,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                RegisteredHours = Math.Round(hours, 2),
                OtType = otType,
                Multiplier = ResolveMultiplier(otType),
                CompensationType = compType,
                Reason = dto.Reason.Trim(),
                Status = OvertimeStatus.Pending,
                CreatedDate = DateOnly.FromDateTime(DateTime.Today)
            };

            await _uow.OvertimeRequestRepository.AddAsync(entity);
            entity.Employee = employee;
            var list = await MapListWithAttendanceAsync(new List<OvertimeRequest> { entity });
            return list.First();
        }

        public async Task<OvertimeRequestDto> ApproveAsync(Guid id, string approvedBy, ReviewOvertimeRequestDto? review = null)
        {
            var entity = await _uow.OvertimeRequestRepository
                .Query()
                .Include(o => o.Employee)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy đơn đăng ký OT.");

            var wasCompensatoryApproved = entity.Status == OvertimeStatus.Approved
                && entity.CompensationType == OvertimeCompensationType.CompensatoryLeave;
            var prevCompHours = wasCompensatoryApproved ? entity.RegisteredHours : 0m;

            if (review?.ApprovedHours.HasValue == true && review.ApprovedHours.Value >= 0.5m)
            {
                entity.RegisteredHours = Math.Round(review.ApprovedHours.Value, 2);
            }
            if (review?.OtType.HasValue == true)
            {
                entity.OtType = review.OtType.Value;
                entity.Multiplier = ResolveMultiplier(entity.OtType);
            }
            if (review?.CompensationType.HasValue == true)
            {
                entity.CompensationType = review.CompensationType.Value;
            }
            if (!string.IsNullOrWhiteSpace(review?.ApproverNote))
            {
                entity.ApproverNote = review.ApproverNote.Trim();
            }

            entity.Status = OvertimeStatus.Approved;
            entity.ApprovedBy = approvedBy;

            await _uow.OvertimeRequestRepository.UpdateAsync(entity.Id, entity);

            var newCompHours = entity.CompensationType == OvertimeCompensationType.CompensatoryLeave
                ? entity.RegisteredHours
                : 0m;

            if (entity.Employee != null && Math.Abs(newCompHours - prevCompHours) >= 0.01m)
            {
                await CreditCompensatoryLeaveAsync(entity.Employee, entity.Date.Year, newCompHours - prevCompHours);
            }

            await SyncAttendanceOvertimeAsync(entity.EmployeeId, entity.Date);

            var list = await MapListWithAttendanceAsync(new List<OvertimeRequest> { entity });
            return list.First();
        }

        public async Task<OvertimeRequestDto> RejectAsync(Guid id, string rejectedBy, ReviewOvertimeRequestDto? review = null)
        {
            var entity = await _uow.OvertimeRequestRepository
                .Query()
                .Include(o => o.Employee)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy đơn đăng ký OT.");

            var wasCompensatoryApproved = entity.Status == OvertimeStatus.Approved
                && entity.CompensationType == OvertimeCompensationType.CompensatoryLeave;

            entity.Status = OvertimeStatus.Rejected;
            entity.ApprovedBy = rejectedBy;
            if (!string.IsNullOrWhiteSpace(review?.ApproverNote))
            {
                entity.ApproverNote = review.ApproverNote.Trim();
            }

            await _uow.OvertimeRequestRepository.UpdateAsync(entity.Id, entity);

            if (wasCompensatoryApproved && entity.Employee != null)
            {
                await CreditCompensatoryLeaveAsync(entity.Employee, entity.Date.Year, -entity.RegisteredHours);
            }

            await SyncAttendanceOvertimeAsync(entity.EmployeeId, entity.Date);

            var list = await MapListWithAttendanceAsync(new List<OvertimeRequest> { entity });
            return list.First();
        }

        public async Task<OvertimeRequestDto> CancelAsync(Guid id, string userIdentifier)
        {
            var entity = await _uow.OvertimeRequestRepository
                .Query()
                .Include(o => o.Employee)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy đơn đăng ký OT.");

            if (entity.Status != OvertimeStatus.Pending)
                throw new InvalidOperationException("Chỉ có thể hủy đơn OT đang ở trạng thái chờ duyệt.");

            entity.Status = OvertimeStatus.Cancelled;
            await _uow.OvertimeRequestRepository.UpdateAsync(entity.Id, entity);

            var list = await MapListWithAttendanceAsync(new List<OvertimeRequest> { entity });
            return list.First();
        }
    }
}
