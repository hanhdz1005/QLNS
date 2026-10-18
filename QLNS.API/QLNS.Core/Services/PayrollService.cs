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
    public class PayrollService : IPayrollService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PayrollService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PayrollDto>> GetAllAsync(int month, int year)
        {
            var salaries = await _uow.SalaryRepository
                .Query()
                .Include(s => s.Employee)
                .Where(s => s.Month == month && s.Year == year)
                .ToListAsync();

            var totalDays = DateTime.DaysInMonth(year, month);
            var standardDays = PayrollCalculator.CalculateStandardWorkingDays(year, month);

            return salaries.Select(s => new PayrollDto
            {
                EmployeeId = s.EmployeeId,
                EmployeeName = s.EmployeeName,
                DepartmentName = s.Employee?.DepartmentName ?? "",
                Month = s.Month,
                Year = s.Year,
                BasicSalary = s.BasicSalary,
                StandardWorkingDays = standardDays,
                ActualWorkDays = s.ActualWorkDays,
                PaidLeaveDays = s.PaidLeaveDays,
                ActualBasicSalary = s.GrossSalary > 0 ? (s.GrossSalary - s.Allowance - s.Bonus - s.OvertimePay) : s.BasicSalary,
                Allowance = s.Allowance,
                OvertimeHours = 0,
                OvertimePay = s.OvertimePay,
                TotalBonus = s.Bonus,
                TotalPenalty = s.Penalty,
                GrossSalary = s.GrossSalary > 0 ? s.GrossSalary : (s.BasicSalary + s.Allowance + s.Bonus + s.OvertimePay),
                Insurance = s.Insurance,
                TaxableIncome = s.GrossSalary > s.Insurance ? (s.GrossSalary - s.Insurance) : 0,
                Tax = s.Tax,
                NetSalary = s.TotalSalary,
                WorkedDays = (int)s.ActualWorkDays,
                TotalDays = totalDays
            }).ToList();
        }

        public async Task<PayrollDto> GeneratePayrollAsync(Guid employeeId, int month, int year, bool saveToDb = true)
        {
            var employee = await _uow.EmployeeRepository
                .Query(e => e.Position, e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var salary = await _uow.SalaryRepository
                .Query()
                .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.Month == month && s.Year == year);

            var baseSalary = salary?.BasicSalary > 0 
                ? salary.BasicSalary 
                : (employee.Position?.BaseSalary ?? 0m);

            var allowance = salary?.Allowance ?? 0m;

            var standardWorkingDays = PayrollCalculator.CalculateStandardWorkingDays(year, month);

            // 1. Dữ liệu chấm công
            var attendances = await _uow.AttendanceRepository
                .Query()
                .Where(a => a.EmployeeId == employeeId && a.Date.Month == month && a.Date.Year == year)
                .ToListAsync();

            var (actualWorkDays, otHours, otPay) = PayrollCalculator.CalculateAttendanceDetails(
                attendances,
                baseSalary,
                standardWorkingDays);

            // 2. Đơn xin nghỉ phép có hưởng lương (Approved)
            var monthStart = new DateOnly(year, month, 1);
            var monthEnd = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            var approvedLeaves = await _uow.LeaveRequestRepository
                .Query()
                .Where(l => l.EmployeeId == employeeId 
                         && l.Status == LeaveStatus.Approved
                         && l.FromDate <= monthEnd 
                         && l.ToDate >= monthStart)
                .ToListAsync();

            var paidLeaveDays = PayrollCalculator.CalculatePaidLeaveDays(approvedLeaves, year, month);

            // 3. Thưởng và Phạt
            var bonusesPenalties = await _uow.BonusPenaltyRepository.GetByEmployeeAndPeriodAsync(employeeId, month, year);
            var totalBonus = bonusesPenalties.Where(bp => bp.Type == BonusPenaltyType.Bonus).Sum(bp => bp.Amount);
            var totalPenalty = bonusesPenalties.Where(bp => bp.Type == BonusPenaltyType.Penalty).Sum(bp => bp.Amount);

            // 4. Tính toán toàn diện theo quy chuẩn doanh nghiệp
            var calc = PayrollCalculator.Calculate(
                baseSalary: baseSalary,
                allowance: allowance,
                standardWorkingDays: standardWorkingDays,
                actualWorkDays: actualWorkDays,
                paidLeaveDays: paidLeaveDays,
                overtimeHours: otHours,
                overtimePay: otPay,
                totalBonus: totalBonus,
                totalPenalty: totalPenalty,
                calculateAttendance: true,
                includeInsurance: true,
                includeTax: true);

            // 5. Tự động lưu/cập nhật vào bảng Salary nếu được yêu cầu
            if (saveToDb)
            {
                if (salary != null)
                {
                    salary.EmployeeName = employee.FullName;
                    salary.BasicSalary = calc.BaseSalary;
                    salary.ActualWorkDays = calc.ActualWorkDays;
                    salary.PaidLeaveDays = calc.PaidLeaveDays;
                    salary.Allowance = calc.Allowance;
                    salary.Bonus = calc.TotalBonus;
                    salary.Penalty = calc.TotalPenalty;
                    salary.OvertimePay = calc.OvertimePay;
                    salary.GrossSalary = calc.GrossSalary;
                    salary.Insurance = calc.Insurance;
                    salary.Tax = calc.PersonalIncomeTax;
                    salary.TotalSalary = calc.NetSalary;

                    await _uow.SalaryRepository.UpdateAsync(salary.Id, salary);
                }
                else
                {
                    var newSalary = new Salary
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = employee.Id,
                        EmployeeName = employee.FullName,
                        Month = month,
                        Year = year,
                        BasicSalary = calc.BaseSalary,
                        ActualWorkDays = calc.ActualWorkDays,
                        PaidLeaveDays = calc.PaidLeaveDays,
                        Allowance = calc.Allowance,
                        Bonus = calc.TotalBonus,
                        Penalty = calc.TotalPenalty,
                        OvertimePay = calc.OvertimePay,
                        GrossSalary = calc.GrossSalary,
                        Insurance = calc.Insurance,
                        Tax = calc.PersonalIncomeTax,
                        TotalSalary = calc.NetSalary
                    };
                    await _uow.SalaryRepository.AddAsync(newSalary);
                }
            }

            return new PayrollDto
            {
                EmployeeId = employeeId,
                EmployeeName = employee.FullName,
                DepartmentName = employee.DepartmentName,
                Month = month,
                Year = year,
                BasicSalary = calc.BaseSalary,
                StandardWorkingDays = calc.StandardWorkingDays,
                ActualWorkDays = calc.ActualWorkDays,
                PaidLeaveDays = calc.PaidLeaveDays,
                ActualBasicSalary = calc.ActualBasicSalary,
                Allowance = calc.Allowance,
                OvertimeHours = calc.OvertimeHours,
                OvertimePay = calc.OvertimePay,
                TotalBonus = calc.TotalBonus,
                TotalPenalty = calc.TotalPenalty,
                GrossSalary = calc.GrossSalary,
                Insurance = calc.Insurance,
                TaxableIncome = calc.TaxableIncome,
                Tax = calc.PersonalIncomeTax,
                NetSalary = calc.NetSalary,
                WorkedDays = (int)calc.ActualWorkDays,
                TotalDays = DateTime.DaysInMonth(year, month)
            };
        }

        public async Task<IEnumerable<PayrollDto>> GenerateBulkPayrollAsync(int month, int year)
        {
            return await GenerateBulkPayrollAsync(new PayrollGenerateDto
            {
                Month = month,
                Year = year,
                CalculateAttendance = true,
                IncludeInsurance = true,
                IncludeTax = true,
                SaveToDatabase = true
            });
        }

        public async Task<IEnumerable<PayrollDto>> GenerateBulkPayrollAsync(PayrollGenerateDto dto)
        {
            var employees = await _uow.EmployeeRepository
                .Query(e => e.Position, e => e.Department)
                .Where(e => e.Status == EmployeeStatus.Active)
                .ToListAsync();

            var existingSalaries = await _uow.SalaryRepository
                .Query()
                .Where(s => s.Month == dto.Month && s.Year == dto.Year)
                .ToListAsync();

            var standardWorkingDays = dto.StandardWorkingDays ?? PayrollCalculator.CalculateStandardWorkingDays(dto.Year, dto.Month);

            var monthStart = new DateOnly(dto.Year, dto.Month, 1);
            var monthEnd = new DateOnly(dto.Year, dto.Month, DateTime.DaysInMonth(dto.Year, dto.Month));

            // Tải dữ liệu trước theo đợt để tối ưu hiệu năng
            var allAttendances = await _uow.AttendanceRepository
                .Query()
                .Where(a => a.Date.Month == dto.Month && a.Date.Year == dto.Year)
                .ToListAsync();

            var allApprovedLeaves = await _uow.LeaveRequestRepository
                .Query()
                .Where(l => l.Status == LeaveStatus.Approved
                         && l.FromDate <= monthEnd 
                         && l.ToDate >= monthStart)
                .ToListAsync();

            var allBonusesPenalties = await _uow.BonusPenaltyRepository.GetByPeriodAsync(dto.Month, dto.Year);
            var totalDays = DateTime.DaysInMonth(dto.Year, dto.Month);

            var results = new List<PayrollDto>();

            foreach (var employee in employees)
            {
                var existingSalary = existingSalaries.FirstOrDefault(s => s.EmployeeId == employee.Id);
                var baseSalary = existingSalary?.BasicSalary > 0
                    ? existingSalary.BasicSalary
                    : (employee.Position?.BaseSalary ?? 0m);

                var allowance = existingSalary?.Allowance ?? 0m;

                var empAttendances = allAttendances.Where(a => a.EmployeeId == employee.Id).ToList();
                var (actualWorkDays, otHours, otPay) = PayrollCalculator.CalculateAttendanceDetails(
                    empAttendances,
                    baseSalary,
                    standardWorkingDays);

                var empLeaves = allApprovedLeaves.Where(l => l.EmployeeId == employee.Id).ToList();
                var paidLeaveDays = PayrollCalculator.CalculatePaidLeaveDays(empLeaves, dto.Year, dto.Month);

                var empBp = allBonusesPenalties.Where(bp => bp.EmployeeId == employee.Id).ToList();
                var totalBonus = empBp.Where(bp => bp.Type == BonusPenaltyType.Bonus).Sum(bp => bp.Amount);
                var totalPenalty = empBp.Where(bp => bp.Type == BonusPenaltyType.Penalty).Sum(bp => bp.Amount);

                var calc = PayrollCalculator.Calculate(
                    baseSalary: baseSalary,
                    allowance: allowance,
                    standardWorkingDays: standardWorkingDays,
                    actualWorkDays: actualWorkDays,
                    paidLeaveDays: paidLeaveDays,
                    overtimeHours: otHours,
                    overtimePay: otPay,
                    totalBonus: totalBonus,
                    totalPenalty: totalPenalty,
                    calculateAttendance: dto.CalculateAttendance,
                    includeInsurance: dto.IncludeInsurance,
                    includeTax: dto.IncludeTax);

                if (dto.SaveToDatabase)
                {
                    if (existingSalary != null)
                    {
                        existingSalary.EmployeeName = employee.FullName;
                        existingSalary.BasicSalary = calc.BaseSalary;
                        existingSalary.ActualWorkDays = calc.ActualWorkDays;
                        existingSalary.PaidLeaveDays = calc.PaidLeaveDays;
                        existingSalary.Allowance = calc.Allowance;
                        existingSalary.Bonus = calc.TotalBonus;
                        existingSalary.Penalty = calc.TotalPenalty;
                        existingSalary.OvertimePay = calc.OvertimePay;
                        existingSalary.GrossSalary = calc.GrossSalary;
                        existingSalary.Insurance = calc.Insurance;
                        existingSalary.Tax = calc.PersonalIncomeTax;
                        existingSalary.TotalSalary = calc.NetSalary;

                        await _uow.SalaryRepository.UpdateAsync(existingSalary.Id, existingSalary);
                    }
                    else
                    {
                        var newSalary = new Salary
                        {
                            Id = Guid.NewGuid(),
                            EmployeeId = employee.Id,
                            EmployeeName = employee.FullName,
                            Month = dto.Month,
                            Year = dto.Year,
                            BasicSalary = calc.BaseSalary,
                            ActualWorkDays = calc.ActualWorkDays,
                            PaidLeaveDays = calc.PaidLeaveDays,
                            Allowance = calc.Allowance,
                            Bonus = calc.TotalBonus,
                            Penalty = calc.TotalPenalty,
                            OvertimePay = calc.OvertimePay,
                            GrossSalary = calc.GrossSalary,
                            Insurance = calc.Insurance,
                            Tax = calc.PersonalIncomeTax,
                            TotalSalary = calc.NetSalary
                        };
                        await _uow.SalaryRepository.AddAsync(newSalary);
                    }
                }

                results.Add(new PayrollDto
                {
                    EmployeeId = employee.Id,
                    EmployeeName = employee.FullName,
                    DepartmentName = employee.DepartmentName,
                    Month = dto.Month,
                    Year = dto.Year,
                    BasicSalary = calc.BaseSalary,
                    StandardWorkingDays = calc.StandardWorkingDays,
                    ActualWorkDays = calc.ActualWorkDays,
                    PaidLeaveDays = calc.PaidLeaveDays,
                    ActualBasicSalary = calc.ActualBasicSalary,
                    Allowance = calc.Allowance,
                    OvertimeHours = calc.OvertimeHours,
                    OvertimePay = calc.OvertimePay,
                    TotalBonus = calc.TotalBonus,
                    TotalPenalty = calc.TotalPenalty,
                    GrossSalary = calc.GrossSalary,
                    Insurance = calc.Insurance,
                    TaxableIncome = calc.TaxableIncome,
                    Tax = calc.PersonalIncomeTax,
                    NetSalary = calc.NetSalary,
                    WorkedDays = (int)calc.ActualWorkDays,
                    TotalDays = totalDays
                });
            }

            return results;
        }

        public async Task<PayrollDto> GetMyPayslipAsync(string userIdentifier, int month, int year)
        {
            var employee = await _uow.EmployeeRepository.GetByIdentifierAsync(userIdentifier);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found for current user");

            return await GeneratePayrollAsync(employee.Id, month, year, false);
        }
    }
}
