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

        private static OvertimeRequestDto MapToDto(OvertimeRequest entity)
        {
            return new OvertimeRequestDto
            {
                Id = entity.Id,
                EmployeeId = entity.EmployeeId,
                EmployeeName = entity.EmployeeName,
                DepartmentName = entity.Employee?.DepartmentName ?? string.Empty,
                Date = entity.Date,
                RegisteredHours = entity.RegisteredHours,
                Reason = entity.Reason,
                Status = entity.Status,
                ApprovedBy = entity.ApprovedBy,
                CreatedDate = entity.CreatedDate
            };
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

            return items.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<OvertimeRequestDto>> GetAllRequestsAsync()
        {
            var items = await _uow.OvertimeRequestRepository
                .Query()
                .Include(o => o.Employee)
                .OrderByDescending(o => o.Date)
                .ThenByDescending(o => o.CreatedDate)
                .ToListAsync();

            return items.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<OvertimeRequestDto>> GetPendingApprovalsAsync()
        {
            var items = await _uow.OvertimeRequestRepository
                .Query()
                .Include(o => o.Employee)
                .Where(o => o.Status == OvertimeStatus.Pending)
                .OrderByDescending(o => o.Date)
                .ToListAsync();

            return items.Select(MapToDto).ToList();
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

            if (dto.RegisteredHours <= 0 || dto.RegisteredHours > 16)
            {
                throw new InvalidOperationException("Số giờ đăng ký OT phải từ 0.5 đến 16 giờ.");
            }

            var entity = new OvertimeRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                EmployeeName = employee.FullName,
                Date = dto.Date,
                RegisteredHours = Math.Round(dto.RegisteredHours, 2),
                Reason = dto.Reason.Trim(),
                Status = OvertimeStatus.Pending,
                CreatedDate = DateOnly.FromDateTime(DateTime.Today)
            };

            await _uow.OvertimeRequestRepository.AddAsync(entity);
            entity.Employee = employee;
            return MapToDto(entity);
        }

        public async Task<OvertimeRequestDto> ApproveAsync(Guid id, string approvedBy)
        {
            var entity = await _uow.OvertimeRequestRepository
                .Query()
                .Include(o => o.Employee)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy đơn đăng ký OT.");

            entity.Status = OvertimeStatus.Approved;
            entity.ApprovedBy = approvedBy;

            await _uow.OvertimeRequestRepository.UpdateAsync(entity.Id, entity);
            await SyncAttendanceOvertimeAsync(entity.EmployeeId, entity.Date);

            return MapToDto(entity);
        }

        public async Task<OvertimeRequestDto> RejectAsync(Guid id, string rejectedBy)
        {
            var entity = await _uow.OvertimeRequestRepository
                .Query()
                .Include(o => o.Employee)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy đơn đăng ký OT.");

            entity.Status = OvertimeStatus.Rejected;
            entity.ApprovedBy = rejectedBy;

            await _uow.OvertimeRequestRepository.UpdateAsync(entity.Id, entity);
            await SyncAttendanceOvertimeAsync(entity.EmployeeId, entity.Date);

            return MapToDto(entity);
        }
    }
}
