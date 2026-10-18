using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNSCore.Entities
{
    public class Salary : BaseEntity<Guid>
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public Employee Employee { get; set; } = null!;
        public decimal BasicSalary { get; set; }
        public decimal ActualWorkDays { get; set; }
        public decimal PaidLeaveDays { get; set; }
        public decimal Allowance { get; set; }
        public decimal Bonus { get; set; }
        public decimal Penalty { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal Insurance { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalSalary { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
