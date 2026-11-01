using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNSCore.Entities
{
    public class Attendance : BaseEntity<Guid>
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public Employee Employee { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal? HoursWorked { get; set; }
        public AttendanceStatus Status { get; set; }
        public string? FaceSnapshotUrl { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal? SimilarityScore { get; set; }
    }

    public enum AttendanceStatus
    {
        Present = 1,
        Absent = 2,
        Late = 3,
        EarlyLeave = 4,
        WorkFromHome = 5,
        Leave = 6,
    }
}
