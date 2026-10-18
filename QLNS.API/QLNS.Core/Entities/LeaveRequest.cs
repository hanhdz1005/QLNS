using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNSCore.Entities
{
    public class LeaveRequest : BaseEntity<Guid>
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } 
        public Employee Employee { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public string Reason { get; set; } 
        public LeaveType LeaveType { get; set; } = LeaveType.AnnualLeave;
        public LeaveStatus Status { get; set; }
        public string? AprovedBy { get; set; } //ManagerName
        public DateOnly CreatedDate { get; set; }
    }

    public enum LeaveType
    {
        AnnualLeave = 1,     // Nghỉ phép năm (có lương, trừ quỹ phép)
        SickLeave = 2,       // Nghỉ ốm đau (hưởng chế độ BHXH)
        MaternityLeave = 3,  // Nghỉ thai sản
        SpecialLeave = 4,    // Nghỉ chế độ (kết hôn, tang chế có lương)
        UnpaidLeave = 5      // Nghỉ không lương
    }

    public enum LeaveStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }
}
