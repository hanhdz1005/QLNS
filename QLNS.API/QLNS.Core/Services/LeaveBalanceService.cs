using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLNS.Core.Dto;
using QLNS.Core.Interfaces;
using QLNS.Core.Interfaces.IService;
using QLNSCore.Entities;
using System;
using System.Threading.Tasks;

namespace QLNS.Core.Services
{
    public class LeaveBalanceService : ILeaveBalanceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public LeaveBalanceService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<LeaveBalanceDto> GetByEmployeeAndYearAsync(Guid employeeId, int year)
        {
            var employee = await _uow.EmployeeRepository.GetAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var balance = await GetOrCreateAsync(employee, year);
            return _mapper.Map<LeaveBalanceDto>(balance);
        }

        public async Task<LeaveBalanceDto> GetMyBalanceAsync(string userIdentifier, int year)
        {
            var employee = await _uow.EmployeeRepository.GetByIdentifierAsync(userIdentifier);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found for current user");

            var balance = await GetOrCreateAsync(employee, year);
            return _mapper.Map<LeaveBalanceDto>(balance);
        }

        public async Task<LeaveBalanceDto> UpdateTotalDaysAsync(Guid employeeId, int year, decimal totalDays)
        {
            var employee = await _uow.EmployeeRepository.GetAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var balance = await GetOrCreateAsync(employee, year);
            balance.TotalLeaveDays = totalDays;
            balance.RemainingLeaveDays = totalDays - balance.UsedLeaveDays;

            await _uow.LeaveBalanceRepository.UpdateAsync(balance.Id, balance);
            return _mapper.Map<LeaveBalanceDto>(balance);
        }

        public async Task DeductLeaveDaysAsync(Guid employeeId, int year, decimal days)
        {
            var employee = await _uow.EmployeeRepository.GetAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var balance = await GetOrCreateAsync(employee, year);
            balance.UsedLeaveDays += days;
            balance.RemainingLeaveDays = balance.TotalLeaveDays - balance.UsedLeaveDays;

            await _uow.LeaveBalanceRepository.UpdateAsync(balance.Id, balance);
        }

        private async Task<LeaveBalance> GetOrCreateAsync(Employee employee, int year)
        {
            var balance = await _uow.LeaveBalanceRepository.GetByEmployeeAndYearAsync(employee.Id, year);
            if (balance != null)
                return balance;

            balance = new LeaveBalance
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                EmployeeName = employee.FullName,
                Year = year,
                TotalLeaveDays = 12m,
                UsedLeaveDays = 0m,
                RemainingLeaveDays = 12m
            };

            await _uow.LeaveBalanceRepository.AddAsync(balance);
            return balance;
        }
    }
}
