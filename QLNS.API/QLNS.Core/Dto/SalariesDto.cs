using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Dto
{
    public class SalariesDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
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

    public class AddSalariesDto
    {
        [Required(ErrorMessage = "EmployeeId is required")]
        public Guid EmployeeId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Allowance must be non-negative")]
        public decimal Allowance { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Bonus must be non-negative")]
        public decimal Bonus { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Penalty must be non-negative")]
        public decimal Penalty { get; set; }

        public decimal ActualWorkDays { get; set; }
        public decimal PaidLeaveDays { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal Insurance { get; set; }
        public decimal Tax { get; set; }

        [Required(ErrorMessage = "Month is required")]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
        public int Month { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100")]
        public int Year { get; set; }
    }
}