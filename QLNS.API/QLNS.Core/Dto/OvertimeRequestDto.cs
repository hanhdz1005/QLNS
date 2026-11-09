using QLNSCore.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace QLNS.Core.Dto
{
    public class OvertimeRequestDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public decimal RegisteredHours { get; set; }
        public string Reason { get; set; } = string.Empty;
        public OvertimeStatus Status { get; set; }
        public string? ApprovedBy { get; set; }
        public DateOnly CreatedDate { get; set; }
    }

    public class AddOvertimeRequestDto
    {
        public Guid? EmployeeId { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        [Range(0.5, 16.0)]
        public decimal RegisteredHours { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
