using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNSCore.Entities
{
    public class OvertimeRequest : BaseEntity<Guid>
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public Employee Employee { get; set; } = null!;
        public DateOnly Date { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal RegisteredHours { get; set; }

        public OvertimeType OtType { get; set; } = OvertimeType.Weekday;

        [Column(TypeName = "decimal(3,2)")]
        public decimal Multiplier { get; set; } = 1.50m;

        public OvertimeCompensationType CompensationType { get; set; } = OvertimeCompensationType.CompensatoryLeave;

        public string Reason { get; set; } = string.Empty;
        public OvertimeStatus Status { get; set; } = OvertimeStatus.Pending;
        public string? ApprovedBy { get; set; }
        public string? ApproverNote { get; set; }
        public DateOnly CreatedDate { get; set; }
    }

    public enum OvertimeStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }

    public enum OvertimeType
    {
        Weekday = 1,  // Ngày thường (150%)
        Weekend = 2,  // Ngày nghỉ hàng tuần (200%)
        Holiday = 3   // Ngày Lễ / Tết (300%)
    }

    public enum OvertimeCompensationType
    {
        CompensatoryLeave = 1, // Quy đổi nghỉ bù (cộng vào quỹ phép năm, không tính lương giờ)
        RecognitionOnly = 2,   // Ghi nhận chuyên cần / đánh giá KPI & xét thưởng (không tính lương giờ)
        Paid = 3               // Tính phụ cấp/tiền lương tăng ca theo giờ
    }
}
