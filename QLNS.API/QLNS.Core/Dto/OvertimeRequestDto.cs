using QLNSCore.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace QLNS.Core.Dto
{
    public class OvertimeRequestDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public decimal RegisteredHours { get; set; }
        public OvertimeType OtType { get; set; }
        public decimal Multiplier { get; set; }
        public OvertimeCompensationType CompensationType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public OvertimeStatus Status { get; set; }
        public string? ApprovedBy { get; set; }
        public string? ApproverNote { get; set; }
        public DateOnly CreatedDate { get; set; }

        // Dữ liệu đối chiếu chấm công khuôn mặt thực tế trong ngày
        public TimeSpan? ActualCheckIn { get; set; }
        public TimeSpan? ActualCheckOut { get; set; }
        public decimal? ActualHoursWorked { get; set; }
    }

    public class AddOvertimeRequestDto
    {
        public Guid? EmployeeId { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        [Required]
        [Range(0.5, 16.0)]
        public decimal RegisteredHours { get; set; }

        public OvertimeType? OtType { get; set; }
        public OvertimeCompensationType? CompensationType { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;
    }

    public class ReviewOvertimeRequestDto
    {
        [Range(0.5, 16.0)]
        public decimal? ApprovedHours { get; set; }
        public OvertimeCompensationType? CompensationType { get; set; }
        public OvertimeType? OtType { get; set; }
        public string? ApproverNote { get; set; }
    }

    public class OvertimeMonthlySummaryDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalApprovedHours { get; set; }
        public decimal MonthlyLimitHours { get; set; } = 40.0m;
        public decimal CompensatoryHours { get; set; }
        public decimal CompensatoryDaysAdded { get; set; }
        public decimal RecognitionHours { get; set; }
        public decimal PaidHours { get; set; }
        public int PendingCount { get; set; }
    }
}
