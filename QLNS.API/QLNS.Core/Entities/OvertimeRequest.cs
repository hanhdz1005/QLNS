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

        [Column(TypeName = "decimal(5,2)")]
        public decimal RegisteredHours { get; set; }

        public string Reason { get; set; } = string.Empty;
        public OvertimeStatus Status { get; set; } = OvertimeStatus.Pending;
        public string? ApprovedBy { get; set; }
        public DateOnly CreatedDate { get; set; }
    }

    public enum OvertimeStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }
}
