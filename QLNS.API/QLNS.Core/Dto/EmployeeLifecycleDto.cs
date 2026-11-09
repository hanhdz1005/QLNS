using QLNSCore.Entities;
using System;
using System.Collections.Generic;

namespace QLNS.Core.Dto
{
    public class LifecycleStageDurationDto
    {
        public string StageKey { get; set; } = string.Empty; // Internship | Probation | Official
        public bool HasCompletedOrActive { get; set; }
        public bool IsCurrent { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal TotalMonths { get; set; }
        public int TotalDays { get; set; }
        public int ContractCount { get; set; }
        public decimal RepresentativeSalary { get; set; }
    }

    public class MonthlyLifecycleMetricDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string Label { get; set; } = string.Empty;
        public decimal CumulativeTenureMonths { get; set; }
        public decimal RegularHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal PunctualityRate { get; set; }
        public decimal NetSalary { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal PenaltyAmount { get; set; }
    }

    public class LifecycleMilestoneDto
    {
        public DateOnly Date { get; set; }
        public string Category { get; set; } = string.Empty; // Onboarding | Internship | Probation | Official | Bonus | Penalty
        public string CodeOrRef { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
    }

    public class EmployeeLifecycleSummaryDto
    {
        public Guid EmployeeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public string? FaceImageUrl { get; set; }
        public DateOnly JoinDate { get; set; }
        public EmployeeStatus Status { get; set; }
        public string CurrentStage { get; set; } = "Official"; // Internship | Probation | Official | Resigned
        public decimal InternshipMonths { get; set; }
        public decimal ProbationMonths { get; set; }
        public decimal OfficialMonths { get; set; }
        public decimal TotalTenureMonths { get; set; }
        public decimal CurrentSalary { get; set; }
        public decimal MonthlyRegularHours { get; set; }
        public decimal MonthlyOvertimeHours { get; set; }
    }

    public class CompanyLifecycleOverviewDto
    {
        public int AsOfMonth { get; set; }
        public int AsOfYear { get; set; }
        public int TotalEmployees { get; set; }
        public int InternshipCount { get; set; }
        public int ProbationCount { get; set; }
        public int OfficialCount { get; set; }
        public int ResignedCount { get; set; }
        public decimal AverageTenureMonths { get; set; }
        public decimal RetentionRate { get; set; }
        public List<EmployeeLifecycleSummaryDto> Employees { get; set; } = new();
    }

    public class EmployeeLifecycleDetailDto
    {
        public Guid EmployeeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public string? ManagerName { get; set; }
        public string? FaceImageUrl { get; set; }
        public DateOnly JoinDate { get; set; }
        public EmployeeStatus Status { get; set; }
        public string CurrentStage { get; set; } = "Official";
        public int AsOfMonth { get; set; }
        public int AsOfYear { get; set; }

        // Liệt kê 3 giai đoạn: Thực tập - Thử việc - Chính thức
        public LifecycleStageDurationDto InternshipStage { get; set; } = new();
        public LifecycleStageDurationDto ProbationStage { get; set; } = new();
        public LifecycleStageDurationDto OfficialStage { get; set; } = new();

        // Chỉ số KPI tự động cập nhật theo tháng
        public decimal TotalTenureMonths { get; set; }
        public int TotalTenureDays { get; set; }
        public decimal InitialSalary { get; set; }
        public decimal CurrentSalary { get; set; }
        public decimal SalaryGrowthPercent { get; set; }
        public decimal CurrentMonthRegularHours { get; set; }
        public decimal CurrentMonthOvertimeHours { get; set; }
        public decimal CurrentMonthWorkDays { get; set; }
        public decimal OverallPunctualityRate { get; set; }
        public int OnTimeDays { get; set; }
        public int LateDays { get; set; }
        public decimal LeaveDaysUsed { get; set; }
        public decimal LeaveDaysRemaining { get; set; }
        public int TotalBonusCount { get; set; }
        public decimal TotalBonusAmount { get; set; }
        public int TotalPenaltyCount { get; set; }
        public decimal TotalPenaltyAmount { get; set; }
        public int TotalContractsCount { get; set; }

        // Chuỗi dữ liệu biểu đồ theo tháng & Trục thời gian
        public List<MonthlyLifecycleMetricDto> MonthlySeries { get; set; } = new();
        public List<LifecycleMilestoneDto> Milestones { get; set; } = new();
    }
}
