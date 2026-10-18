using System;
using System.ComponentModel.DataAnnotations;

namespace QLNS.Core.Dto
{
    public class LeaveBalanceDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal TotalLeaveDays { get; set; }
        public decimal UsedLeaveDays { get; set; }
        public decimal RemainingLeaveDays { get; set; }
    }

    public class UpdateLeaveBalanceDto
    {
        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "TotalLeaveDays must be between 0 and 100")]
        public decimal TotalLeaveDays { get; set; }
    }
}
