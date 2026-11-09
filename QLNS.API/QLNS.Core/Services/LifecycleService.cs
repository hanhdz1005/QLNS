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
    public class LifecycleService : ILifecycleService
    {
        private readonly IUnitOfWork _uow;

        public LifecycleService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        private static (LifecycleStageDurationDto Internship, LifecycleStageDurationDto Probation, LifecycleStageDurationDto Official, string CurrentStage, decimal TotalMonths, int TotalDays)
            CalculateStageDurations(Employee emp, List<Contract> contracts, decimal baseSalary, DateOnly asOfDate)
        {
            var joinDate = emp.JoinDate;
            if (asOfDate < joinDate) asOfDate = joinDate;

            var totalDays = Math.Max(1, asOfDate.DayNumber - joinDate.DayNumber);
            var totalMonths = Math.Round(totalDays / 30.0m, 1);

            var internContracts = contracts.Where(c => c.ContractType == ContractType.Internship).OrderBy(c => c.StartDate).ToList();
            var probContracts = contracts.Where(c => c.ContractType == ContractType.Probation).OrderBy(c => c.StartDate).ToList();
            var officialContracts = contracts
                .Where(c => c.ContractType == ContractType.DefiniteTerm || c.ContractType == ContractType.IndefiniteTerm || c.ContractType == ContractType.PartTime)
                .OrderBy(c => c.StartDate)
                .ToList();

            DateOnly internStart = internContracts.FirstOrDefault()?.StartDate ?? joinDate;
            int internDays = internContracts.Count > 0
                ? internContracts.Sum(c => Math.Max(15, ((c.EndDate ?? asOfDate) < asOfDate ? (c.EndDate ?? asOfDate) : asOfDate).DayNumber - c.StartDate.DayNumber))
                : Math.Min(totalDays, 30);
            DateOnly internEnd = internStart.AddDays(internDays);

            DateOnly probStart = probContracts.FirstOrDefault()?.StartDate ?? internEnd;
            int probDays = probContracts.Count > 0
                ? probContracts.Sum(c => Math.Max(15, ((c.EndDate ?? asOfDate) < asOfDate ? (c.EndDate ?? asOfDate) : asOfDate).DayNumber - c.StartDate.DayNumber))
                : (totalDays > 30 ? Math.Min(totalDays - internDays, 60) : 0);
            DateOnly probEnd = probStart.AddDays(probDays);

            int officialDays = Math.Max(0, totalDays - internDays - probDays);
            DateOnly? officialStart = officialDays > 0 ? (officialContracts.FirstOrDefault()?.StartDate ?? probEnd) : null;

            string currentStage = emp.Status == EmployeeStatus.Resigned
                ? "Resigned"
                : emp.Status == EmployeeStatus.Probation || (officialDays == 0 && probDays > 0)
                    ? "Probation"
                    : officialDays == 0 && internDays > 0 ? "Internship" : "Official";

            var internDto = new LifecycleStageDurationDto
            {
                StageKey = "Internship",
                HasCompletedOrActive = internDays > 0,
                IsCurrent = currentStage == "Internship",
                StartDate = internStart,
                EndDate = internEnd,
                TotalDays = internDays,
                TotalMonths = Math.Round(internDays / 30.0m, 1),
                ContractCount = Math.Max(1, internContracts.Count),
                RepresentativeSalary = internContracts.LastOrDefault()?.ContractSalary ?? Math.Round(baseSalary * 0.5m, 0)
            };

            var probDto = new LifecycleStageDurationDto
            {
                StageKey = "Probation",
                HasCompletedOrActive = probDays > 0,
                IsCurrent = currentStage == "Probation",
                StartDate = probDays > 0 ? probStart : null,
                EndDate = probDays > 0 ? probEnd : null,
                TotalDays = probDays,
                TotalMonths = Math.Round(probDays / 30.0m, 1),
                ContractCount = Math.Max(probDays > 0 ? 1 : 0, probContracts.Count),
                RepresentativeSalary = probContracts.LastOrDefault()?.ContractSalary ?? Math.Round(baseSalary * 0.85m, 0)
            };

            var officialDto = new LifecycleStageDurationDto
            {
                StageKey = "Official",
                HasCompletedOrActive = officialDays > 0,
                IsCurrent = currentStage == "Official",
                StartDate = officialStart,
                EndDate = officialDays > 0 ? asOfDate : null,
                TotalDays = officialDays,
                TotalMonths = Math.Round(officialDays / 30.0m, 1),
                ContractCount = Math.Max(officialDays > 0 ? 1 : 0, officialContracts.Count),
                RepresentativeSalary = officialContracts.LastOrDefault()?.ContractSalary ?? baseSalary
            };

            return (internDto, probDto, officialDto, currentStage, totalMonths, totalDays);
        }

        public async Task<CompanyLifecycleOverviewDto> GetCompanyOverviewAsync(int month, int year)
        {
            var asOfDate = new DateOnly(year, month, DateTime.DaysInMonth(year, month));
            var employees = await _uow.EmployeeRepository
                .Query(e => e.Department, e => e.Position, e => e.Contracts)
                .OrderBy(e => e.Code)
                .ToListAsync();

            var monthAttendances = await _uow.AttendanceRepository.Query()
                .Where(a => a.Date.Month == month && a.Date.Year == year).ToListAsync();

            var monthApprovedOt = await _uow.OvertimeRequestRepository.Query()
                .Where(o => o.Date.Month == month && o.Date.Year == year && o.Status == OvertimeStatus.Approved).ToListAsync();

            var summaries = new List<EmployeeLifecycleSummaryDto>();
            foreach (var emp in employees)
            {
                var baseSalary = emp.Contracts?.OrderByDescending(c => c.StartDate).FirstOrDefault()?.ContractSalary
                    ?? emp.Position?.BaseSalary ?? 15000000m;

                var (intern, prob, official, currentStage, totalMonths, _) = CalculateStageDurations(
                    emp, emp.Contracts?.ToList() ?? new List<Contract>(), baseSalary, asOfDate);

                var empAtts = monthAttendances.Where(a => a.EmployeeId == emp.Id).ToList();
                var regHours = empAtts.Sum(a => a.RegularHours ?? Math.Min(8.0m, a.HoursWorked ?? 8.0m));
                var otHours = monthApprovedOt.Where(o => o.EmployeeId == emp.Id).Sum(o => o.RegisteredHours);

                summaries.Add(new EmployeeLifecycleSummaryDto
                {
                    EmployeeId = emp.Id,
                    Code = emp.Code,
                    FullName = emp.FullName,
                    DepartmentName = emp.Department?.DepartmentName ?? emp.DepartmentName,
                    PositionName = emp.Position?.PositionName ?? emp.PositionName,
                    FaceImageUrl = emp.FaceImageUrl,
                    JoinDate = emp.JoinDate,
                    Status = emp.Status,
                    CurrentStage = currentStage,
                    InternshipMonths = intern.TotalMonths,
                    ProbationMonths = prob.TotalMonths,
                    OfficialMonths = official.TotalMonths,
                    TotalTenureMonths = totalMonths,
                    CurrentSalary = official.RepresentativeSalary,
                    MonthlyRegularHours = Math.Round(regHours, 1),
                    MonthlyOvertimeHours = Math.Round(otHours, 1)
                });
            }

            var totalCount = summaries.Count;
            var resignedCount = summaries.Count(s => s.CurrentStage == "Resigned");

            return new CompanyLifecycleOverviewDto
            {
                AsOfMonth = month,
                AsOfYear = year,
                TotalEmployees = totalCount,
                InternshipCount = summaries.Count(s => s.CurrentStage == "Internship"),
                ProbationCount = summaries.Count(s => s.CurrentStage == "Probation"),
                OfficialCount = summaries.Count(s => s.CurrentStage == "Official"),
                ResignedCount = resignedCount,
                AverageTenureMonths = totalCount > 0 ? Math.Round(summaries.Average(s => s.TotalTenureMonths), 1) : 0m,
                RetentionRate = totalCount > 0 ? Math.Round(((totalCount - resignedCount) * 100.0m) / totalCount, 1) : 100m,
                Employees = summaries
            };
        }

        public async Task<EmployeeLifecycleDetailDto> GetMyLifecycleAsync(string userIdentifier, int month, int year)
        {
            var employee = await _uow.EmployeeRepository.GetByIdentifierAsync(userIdentifier)
                ?? await _uow.EmployeeRepository.Query().OrderBy(e => e.Code).FirstOrDefaultAsync();
            if (employee == null) throw new KeyNotFoundException("Không tìm thấy hồ sơ nhân viên.");
            return await GetEmployeeLifecycleAsync(employee.Id, month, year);
        }

        public async Task<EmployeeLifecycleDetailDto> GetEmployeeLifecycleAsync(Guid employeeId, int month, int year)
        {
            var emp = await _uow.EmployeeRepository
                .Query(e => e.Department, e => e.Position, e => e.Manager)
                .FirstOrDefaultAsync(e => e.Id == employeeId)
                ?? throw new KeyNotFoundException("Không tìm thấy hồ sơ nhân viên.");

            var contracts = await _uow.ContractRepository.Query().Where(c => c.EmployeeId == employeeId).OrderBy(c => c.StartDate).ToListAsync();
            var salaries = await _uow.SalaryRepository.Query().Where(s => s.EmployeeId == employeeId).ToListAsync();
            var attendances = await _uow.AttendanceRepository.Query().Where(a => a.EmployeeId == employeeId).ToListAsync();
            var approvedOts = await _uow.OvertimeRequestRepository.Query().Where(o => o.EmployeeId == employeeId && o.Status == OvertimeStatus.Approved).ToListAsync();
            var bonusPenalties = await _uow.BonusPenaltyRepository.Query().Where(b => b.EmployeeId == employeeId).OrderByDescending(b => b.Date).ToListAsync();
            var leaveBalance = await _uow.LeaveBalanceRepository.Query().FirstOrDefaultAsync(l => l.EmployeeId == employeeId && l.Year == year);

            var asOfDate = new DateOnly(year, month, DateTime.DaysInMonth(year, month));
            var baseSalary = contracts.LastOrDefault()?.ContractSalary
                ?? salaries.OrderByDescending(s => s.Year).ThenByDescending(s => s.Month).FirstOrDefault()?.BasicSalary
                ?? emp.Position?.BaseSalary ?? 15000000m;

            var (intern, prob, official, currentStage, totalMonths, totalDays) = CalculateStageDurations(emp, contracts, baseSalary, asOfDate);

            var initialSalary = contracts.FirstOrDefault()?.ContractSalary ?? prob.RepresentativeSalary;
            var currentSalary = official.RepresentativeSalary;
            var growthPercent = initialSalary > 0 ? Math.Round(((currentSalary - initialSalary) / initialSalary) * 100.0m, 1) : 0m;

            var monthlySeries = new List<MonthlyLifecycleMetricDto>();
            for (int offset = 5; offset >= 0; offset--)
            {
                var targetDt = new DateTime(year, month, 1).AddMonths(-offset);
                int m = targetDt.Month, y = targetDt.Year;
                var endOfM = new DateOnly(y, m, DateTime.DaysInMonth(y, m));
                var cumDays = Math.Max(0, endOfM.DayNumber - emp.JoinDate.DayNumber);

                var mAtts = attendances.Where(a => a.Date.Month == m && a.Date.Year == y).ToList();
                var mRegHours = mAtts.Sum(a => a.RegularHours ?? Math.Min(8.0m, a.HoursWorked ?? 8.0m));
                var mOtHours = approvedOts.Where(o => o.Date.Month == m && o.Date.Year == y).Sum(o => o.RegisteredHours);
                var mOnTime = mAtts.Count(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.WorkFromHome);

                monthlySeries.Add(new MonthlyLifecycleMetricDto
                {
                    Month = m,
                    Year = y,
                    Label = $"{m:D2}/{y}",
                    CumulativeTenureMonths = Math.Round(cumDays / 30.0m, 1),
                    RegularHours = Math.Round(mRegHours, 1),
                    OvertimeHours = Math.Round(mOtHours, 1),
                    PunctualityRate = mAtts.Count > 0 ? Math.Round((mOnTime * 100.0m) / mAtts.Count, 1) : 100.0m,
                    NetSalary = salaries.FirstOrDefault(s => s.Month == m && s.Year == y)?.TotalSalary ?? (cumDays > 0 ? currentSalary : 0m),
                    BonusAmount = bonusPenalties.Where(b => b.Month == m && b.Year == y && b.Type == BonusPenaltyType.Bonus).Sum(b => b.Amount),
                    PenaltyAmount = bonusPenalties.Where(b => b.Month == m && b.Year == y && b.Type == BonusPenaltyType.Penalty).Sum(b => b.Amount)
                });
            }

            var milestones = new List<LifecycleMilestoneDto>
            {
                new LifecycleMilestoneDto
                {
                    Date = emp.JoinDate,
                    Category = "Onboarding",
                    CodeOrRef = emp.Code,
                    Description = $"{emp.Department?.DepartmentName ?? emp.DepartmentName} - {emp.Position?.PositionName ?? emp.PositionName}",
                    Amount = initialSalary
                }
            };

            if (intern.HasCompletedOrActive && intern.StartDate.HasValue)
            {
                milestones.Add(new LifecycleMilestoneDto { Date = intern.StartDate.Value, Category = "Internship", CodeOrRef = $"{intern.TotalMonths}M", Description = $"{intern.StartDate:yyyy-MM-dd} -> {intern.EndDate:yyyy-MM-dd}", Amount = intern.RepresentativeSalary });
            }
            if (prob.HasCompletedOrActive && prob.StartDate.HasValue)
            {
                milestones.Add(new LifecycleMilestoneDto { Date = prob.StartDate.Value, Category = "Probation", CodeOrRef = $"{prob.TotalMonths}M", Description = $"{prob.StartDate:yyyy-MM-dd} -> {prob.EndDate:yyyy-MM-dd}", Amount = prob.RepresentativeSalary });
            }
            foreach (var c in contracts)
            {
                milestones.Add(new LifecycleMilestoneDto { Date = c.StartDate, Category = c.ContractType == ContractType.Probation ? "Probation" : c.ContractType == ContractType.Internship ? "Internship" : "Official", CodeOrRef = c.ContractNumber, Description = c.Notes ?? c.ContractNumber, Amount = c.ContractSalary });
            }
            foreach (var bp in bonusPenalties)
            {
                milestones.Add(new LifecycleMilestoneDto { Date = bp.Date, Category = bp.Type == BonusPenaltyType.Bonus ? "Bonus" : "Penalty", CodeOrRef = $"{bp.Month:D2}/{bp.Year}", Description = bp.Reason, Amount = bp.Amount });
            }

            var currentMonthSummary = monthlySeries.Last();
            var totalOnTime = attendances.Count(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.WorkFromHome);
            var totalLate = attendances.Count(a => a.Status == AttendanceStatus.Late || a.Status == AttendanceStatus.EarlyLeave);

            return new EmployeeLifecycleDetailDto
            {
                EmployeeId = emp.Id,
                Code = emp.Code,
                FullName = emp.FullName,
                Email = emp.Email,
                PhoneNumber = emp.PhoneNumber,
                DepartmentName = emp.Department?.DepartmentName ?? emp.DepartmentName,
                PositionName = emp.Position?.PositionName ?? emp.PositionName,
                ManagerName = emp.Manager?.FullName,
                FaceImageUrl = emp.FaceImageUrl,
                JoinDate = emp.JoinDate,
                Status = emp.Status,
                CurrentStage = currentStage,
                AsOfMonth = month,
                AsOfYear = year,
                InternshipStage = intern,
                ProbationStage = prob,
                OfficialStage = official,
                TotalTenureMonths = totalMonths,
                TotalTenureDays = totalDays,
                InitialSalary = initialSalary,
                CurrentSalary = currentSalary,
                SalaryGrowthPercent = growthPercent,
                CurrentMonthRegularHours = currentMonthSummary.RegularHours,
                CurrentMonthOvertimeHours = currentMonthSummary.OvertimeHours,
                CurrentMonthWorkDays = Math.Round(currentMonthSummary.RegularHours / 8.0m, 1),
                OverallPunctualityRate = attendances.Count > 0 ? Math.Round((totalOnTime * 100.0m) / attendances.Count, 1) : 100.0m,
                OnTimeDays = totalOnTime,
                LateDays = totalLate,
                LeaveDaysUsed = leaveBalance?.UsedLeaveDays ?? 0m,
                LeaveDaysRemaining = leaveBalance?.RemainingLeaveDays ?? 12m,
                TotalBonusCount = bonusPenalties.Count(b => b.Type == BonusPenaltyType.Bonus),
                TotalBonusAmount = bonusPenalties.Where(b => b.Type == BonusPenaltyType.Bonus).Sum(b => b.Amount),
                TotalPenaltyCount = bonusPenalties.Count(b => b.Type == BonusPenaltyType.Penalty),
                TotalPenaltyAmount = bonusPenalties.Where(b => b.Type == BonusPenaltyType.Penalty).Sum(b => b.Amount),
                TotalContractsCount = Math.Max(1, contracts.Count),
                MonthlySeries = monthlySeries,
                Milestones = milestones.OrderByDescending(m => m.Date).ToList()
            };
        }
    }
}
