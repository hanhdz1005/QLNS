using System;
using System.Collections.Generic;

namespace QLNS.Core.Dto
{
    public class BonusPenaltyDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int Type { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public DateOnly Date { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class AddBonusPenaltyDto
    {
        public Guid EmployeeId { get; set; }
        public int Type { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public DateOnly Date { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class PayrollDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BasicSalary { get; set; }
        public int StandardWorkingDays { get; set; }
        public decimal ActualWorkDays { get; set; }
        public decimal PaidLeaveDays { get; set; }
        public decimal ActualBasicSalary { get; set; }
        public decimal Allowance { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalPenalty { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal Insurance { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal Tax { get; set; }
        public decimal NetSalary { get; set; }
        public int WorkedDays { get; set; }
        public int TotalDays { get; set; }
    }

    public class PayrollGenerateDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public bool CalculateAttendance { get; set; } = true;
        public int? StandardWorkingDays { get; set; }
        public bool IncludeInsurance { get; set; } = true;
        public bool IncludeTax { get; set; } = true;
        public bool SaveToDatabase { get; set; } = true;
    }

    public class RefreshTokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class DashboardDto
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int PendingLeaveRequests { get; set; }
        public int TotalAttendancesThisMonth { get; set; }
        public decimal TotalSalaryLastMonth { get; set; }
        public decimal AverageSalary { get; set; }
    }

    public class EmployeeStatsDto
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int EmployeeCount { get; set; }
    }

    public class LeaveReportDto
    {
        public int TotalRequests { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Cancelled { get; set; }
    }

    public class SalaryReportDto
    {
        public int TotalRecords { get; set; }
        public decimal TotalBasicSalary { get; set; }
        public decimal TotalNetSalary { get; set; }
        public decimal AverageNetSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public decimal MinSalary { get; set; }
    }

    public class QrCheckInDto
    {
        public string QrToken { get; set; } = string.Empty;
    }

    public class QrCheckInResultDto
    {
        public bool Success { get; set; } = true;
        public string ActionType { get; set; } = string.Empty; // "CheckIn" hoặc "CheckOut"
        public string Message { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public TimeSpan Time { get; set; }
        public decimal? HoursWorked { get; set; }
    }

    public class HrAlertsDto
    {
        public List<ExpiringContractItemDto> ExpiringContracts { get; set; } = new();
        public List<BirthdayItemDto> UpcomingBirthdays { get; set; } = new();
        public List<ProbationEndingItemDto> ProbationEndingEmployees { get; set; } = new();
    }

    public class ExpiringContractItemDto
    {
        public Guid ContractId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string ContractNumber { get; set; } = string.Empty;
        public DateOnly? EndDate { get; set; }
        public int DaysRemaining { get; set; }
    }

    public class BirthdayItemDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public int Day { get; set; }
    }

    public class ProbationEndingItemDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateOnly JoinDate { get; set; }
        public DateOnly EstimatedProbationEnd { get; set; }
        public int DaysRemaining { get; set; }
    }
}
