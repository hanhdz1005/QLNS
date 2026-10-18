using QLNSCore.Entities;
using System.ComponentModel.DataAnnotations;

namespace QLNS.Core.Dto
{
    public class AttendanceDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }
        public decimal? HoursWorked { get; set; }
        public AttendanceStatus Status { get; set; }
    }

    public class AddAttendanceDto
    {
        [Required(ErrorMessage = "EmployeeId is required")]
        public Guid EmployeeId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateOnly Date { get; set; }

        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public AttendanceStatus Status { get; set; }
    }
}
