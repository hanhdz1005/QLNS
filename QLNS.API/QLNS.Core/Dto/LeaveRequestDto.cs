using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Dto
{
    public class LeaveRequestDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public string Reason { get; set; }
        public LeaveType LeaveType { get; set; }
        public LeaveStatus Status { get; set; }
        public string? AprovedBy { get; set; }
        public DateOnly CreatedDate { get; set; }
    }

    public class AddLeaveRequestDto
    {
        [Required(ErrorMessage = "EmployeeId is required")]
        public Guid EmployeeId { get; set; }

        [Required(ErrorMessage = "FromDate is required")]
        public DateOnly FromDate { get; set; }

        [Required(ErrorMessage = "ToDate is required")]
        public DateOnly ToDate { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Reason must be between 5 and 500 characters")]
        public string Reason { get; set; }

        public LeaveType LeaveType { get; set; } = LeaveType.AnnualLeave;
    }
}

