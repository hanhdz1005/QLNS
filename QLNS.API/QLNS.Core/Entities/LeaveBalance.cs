using System;

namespace QLNSCore.Entities
{
    public class LeaveBalance : BaseEntity<Guid>
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public Employee Employee { get; set; } = null!;

        public int Year { get; set; }
        public decimal TotalLeaveDays { get; set; } = 12m;     // Tiêu chuẩn 12 ngày phép năm + thâm niên
        public decimal UsedLeaveDays { get; set; } = 0m;      // Số ngày đã nghỉ
        public decimal RemainingLeaveDays { get; set; } = 12m; // Số ngày còn lại
    }
}
