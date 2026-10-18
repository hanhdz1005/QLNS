using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLNS.Core.Common;
using QLNS.Core.Interfaces.IService;
using QLNS.Core.Dto;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Services
{
    public class SalaryService : ISalaryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public SalaryService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SalariesDto>> GetAllAsync()
        {
            var items = await _uow.SalaryRepository
                .Query()
                .Include(s => s.Employee)
                .OrderBy(s => s.Month)
                .ToListAsync();
            return _mapper.Map<IReadOnlyList<SalariesDto>>(items);
        }

        public async Task<PagedResult<SalariesDto>> GetFilterAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _uow.SalaryRepository
                .Query()
                .Include(s => s.Employee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(s => s.EmployeeName.ToLower().Contains(search));
            }

            return await query.OrderBy(s => s.Month).ToPagedResultAsync<Salary, SalariesDto>(pageIndex, pageSize, _mapper);
        }

        public async Task<SalariesDto> GetByIdAsync(Guid id)
        {
            var item = await _uow.SalaryRepository
                .Query()
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (item == null)
                throw new KeyNotFoundException("Not found salary");
            return _mapper.Map<SalariesDto>(item);
        }

        public async Task<SalariesDto> AddAsync(AddSalariesDto dto)
        {
            var employee = await _uow.EmployeeRepository.GetByIdAsync(dto.EmployeeId, e => e.Position);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var entity = _mapper.Map<Salary>(dto);
            entity.EmployeeName = employee.FullName;
            entity.BasicSalary = employee.Position.BaseSalary;

            var standardDays = Common.PayrollCalculator.CalculateStandardWorkingDays(dto.Year, dto.Month);
            var actualWorkDays = dto.ActualWorkDays > 0 ? dto.ActualWorkDays : standardDays;

            var calc = Common.PayrollCalculator.Calculate(
                baseSalary: entity.BasicSalary,
                allowance: entity.Allowance,
                standardWorkingDays: standardDays,
                actualWorkDays: actualWorkDays,
                paidLeaveDays: dto.PaidLeaveDays,
                overtimeHours: 0,
                overtimePay: dto.OvertimePay,
                totalBonus: entity.Bonus,
                totalPenalty: entity.Penalty,
                calculateAttendance: dto.ActualWorkDays > 0,
                includeInsurance: true,
                includeTax: true);

            entity.ActualWorkDays = calc.ActualWorkDays;
            entity.PaidLeaveDays = calc.PaidLeaveDays;
            entity.OvertimePay = calc.OvertimePay;
            entity.GrossSalary = calc.GrossSalary;
            entity.Insurance = dto.Insurance > 0 ? dto.Insurance : calc.Insurance;
            entity.Tax = dto.Tax > 0 ? dto.Tax : calc.PersonalIncomeTax;
            entity.TotalSalary = calc.NetSalary;

            await _uow.SalaryRepository.AddAsync(entity);
            return _mapper.Map<SalariesDto>(entity);
        }

        public async Task<SalariesDto> UpdateAsync(Guid id, AddSalariesDto dto)
        {
            var exist = await _uow.SalaryRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found salary");

            var employee = await _uow.EmployeeRepository.GetByIdAsync(dto.EmployeeId, e => e.Position);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var entity = _mapper.Map<Salary>(dto);
            entity.EmployeeName = employee.FullName;
            entity.BasicSalary = employee.Position.BaseSalary;

            var standardDays = Common.PayrollCalculator.CalculateStandardWorkingDays(dto.Year, dto.Month);
            var actualWorkDays = dto.ActualWorkDays > 0 ? dto.ActualWorkDays : standardDays;

            var calc = Common.PayrollCalculator.Calculate(
                baseSalary: entity.BasicSalary,
                allowance: entity.Allowance,
                standardWorkingDays: standardDays,
                actualWorkDays: actualWorkDays,
                paidLeaveDays: dto.PaidLeaveDays,
                overtimeHours: 0,
                overtimePay: dto.OvertimePay,
                totalBonus: entity.Bonus,
                totalPenalty: entity.Penalty,
                calculateAttendance: dto.ActualWorkDays > 0,
                includeInsurance: true,
                includeTax: true);

            entity.ActualWorkDays = calc.ActualWorkDays;
            entity.PaidLeaveDays = calc.PaidLeaveDays;
            entity.OvertimePay = calc.OvertimePay;
            entity.GrossSalary = calc.GrossSalary;
            entity.Insurance = dto.Insurance > 0 ? dto.Insurance : calc.Insurance;
            entity.Tax = dto.Tax > 0 ? dto.Tax : calc.PersonalIncomeTax;
            entity.TotalSalary = calc.NetSalary;
            entity.Id = id;

            await _uow.SalaryRepository.UpdateAsync(id, entity);
            return _mapper.Map<SalariesDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var exist = await _uow.SalaryRepository.GetAsync(id);
            if (exist == null)
                throw new KeyNotFoundException("Not found salary");
            await _uow.SalaryRepository.DeleteAsync(id);
            return true;
        }
    }
}