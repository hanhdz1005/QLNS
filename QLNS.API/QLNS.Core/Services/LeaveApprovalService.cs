using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLNS.Core.Common;
using QLNS.Core.Interfaces;
using QLNS.Core.Interfaces.IService;
using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QLNS.Core.Services
{
    public class LeaveApprovalService : ILeaveApprovalService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public LeaveApprovalService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<LeaveRequestDto> ApproveAsync(Guid id, string approvedBy)
        {
            var entity = await _uow.LeaveRequestRepository.GetAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Leave request not found");

            if (entity.Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be approved");

            entity.Status = LeaveStatus.Approved;
            entity.AprovedBy = approvedBy;
            await _uow.LeaveRequestRepository.UpdateAsync(id, entity);

            if (entity.LeaveType == LeaveType.AnnualLeave)
            {
                var year = entity.FromDate.Year;
                var balance = await _uow.LeaveBalanceRepository.GetByEmployeeAndYearAsync(entity.EmployeeId, year);
                var days = (decimal)(entity.ToDate.DayNumber - entity.FromDate.DayNumber + 1);
                if (balance == null)
                {
                    var employee = await _uow.EmployeeRepository.GetAsync(entity.EmployeeId);
                    balance = new LeaveBalance
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = entity.EmployeeId,
                        EmployeeName = employee?.FullName ?? "",
                        Year = year,
                        TotalLeaveDays = 12m,
                        UsedLeaveDays = days,
                        RemainingLeaveDays = 12m - days
                    };
                    await _uow.LeaveBalanceRepository.AddAsync(balance);
                }
                else
                {
                    balance.UsedLeaveDays += days;
                    balance.RemainingLeaveDays = balance.TotalLeaveDays - balance.UsedLeaveDays;
                    await _uow.LeaveBalanceRepository.UpdateAsync(balance.Id, balance);
                }
            }

            return _mapper.Map<LeaveRequestDto>(entity);
        }

        public async Task<LeaveRequestDto> RejectAsync(Guid id, string rejectedBy)
        {
            var entity = await _uow.LeaveRequestRepository.GetAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Leave request not found");

            if (entity.Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be rejected");

            entity.Status = LeaveStatus.Rejected;
            entity.AprovedBy = rejectedBy;
            await _uow.LeaveRequestRepository.UpdateAsync(id, entity);
            return _mapper.Map<LeaveRequestDto>(entity);
        }

        public async Task<PagedResult<LeaveRequestDto>> GetPendingApprovalsAsync(int pageIndex, int pageSize)
        {
            var query = _uow.LeaveRequestRepository.Query()
                .Where(l => l.Status == LeaveStatus.Pending);

            return await query
                .OrderByDescending(l => l.CreatedDate)
                .ToPagedResultAsync<LeaveRequest, LeaveRequestDto>(pageIndex, pageSize, _mapper);
        }
    }
}
