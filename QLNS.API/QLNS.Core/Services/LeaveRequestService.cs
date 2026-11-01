using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public LeaveRequestService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LeaveRequestDto>> GetAllAsync()
        {
            var items = await _uow.LeaveRequestRepository
                .Query()
                .Include(l => l.Employee)
                .OrderByDescending(l => l.FromDate)
                .ToListAsync();
            return _mapper.Map<IReadOnlyList<LeaveRequestDto>>(items);
        }

        public async Task<PagedResult<LeaveRequestDto>> GetFilterAsync(
            int pageIndex,
            int pageSize,
            string? search,
            string? userIdentifier = null)
        {
            var query = _uow.LeaveRequestRepository
                .Query()
                .Include(l => l.Employee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(userIdentifier))
            {
                var employee = await _uow.EmployeeRepository.GetByIdentifierAsync(userIdentifier);
                if (employee != null)
                {
                    query = query.Where(l => l.EmployeeId == employee.Id);
                }
                else
                {
                    query = query.Where(_ => false);
                }
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(l => l.EmployeeName.ToLower().Contains(search));
            }

            return await query
                .OrderByDescending(l => l.FromDate)
                .ToPagedResultAsync<LeaveRequest, LeaveRequestDto>(pageIndex, pageSize, _mapper);
        }

        public async Task<LeaveRequestDto> GetByIdAsync(Guid id)
        {
            var item = await _uow.LeaveRequestRepository
                .Query()
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.Id == id);
            if (item == null)
                throw new KeyNotFoundException("Not found leave request");
            return _mapper.Map<LeaveRequestDto>(item);
        }

        public async Task<LeaveRequestDto> AddAsync(AddLeaveRequestDto dto)
        {
            if (dto.ToDate < dto.FromDate)
                throw new InvalidOperationException("ToDate must be greater than or equal to FromDate");

            var employee = await _uow.EmployeeRepository.GetAsync(dto.EmployeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var entity = new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = dto.EmployeeId,
                EmployeeName = employee.FullName,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                Reason = dto.Reason,
                LeaveType = dto.LeaveType,
                Status = LeaveStatus.Pending,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now)
            };

            await _uow.LeaveRequestRepository.AddAsync(entity);
            return _mapper.Map<LeaveRequestDto>(entity);
        }

        public async Task<LeaveRequestDto> UpdateAsync(Guid id, AddLeaveRequestDto dto)
        {
            var exist = await _uow.LeaveRequestRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found leave request");

            if (dto.ToDate < dto.FromDate)
                throw new InvalidOperationException("ToDate must be greater than or equal to FromDate");

            var employee = await _uow.EmployeeRepository.GetAsync(dto.EmployeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var entity = _mapper.Map<LeaveRequest>(dto);
            entity.EmployeeName = employee.FullName;
            entity.Id = id;
            await _uow.LeaveRequestRepository.UpdateAsync(id, entity);
            return _mapper.Map<LeaveRequestDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var exist = await _uow.LeaveRequestRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found leave request");
            await _uow.LeaveRequestRepository.DeleteAsync(id);
            return true;
        }
    }
}
