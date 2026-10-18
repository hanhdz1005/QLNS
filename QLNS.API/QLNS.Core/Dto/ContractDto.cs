using QLNSCore.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace QLNS.Core.Dto
{
    public class ContractDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;

        public string ContractNumber { get; set; } = string.Empty;
        public ContractType ContractType { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateOnly SignDate { get; set; }

        public decimal ContractSalary { get; set; }
        public decimal InsuranceSalary { get; set; }
        public string? AttachmentUrl { get; set; }
        public ContractStatus Status { get; set; }
        public string? Notes { get; set; }
    }

    public class AddContractDto
    {
        [Required(ErrorMessage = "EmployeeId is required")]
        public Guid EmployeeId { get; set; }

        [Required(ErrorMessage = "ContractNumber is required")]
        [StringLength(100, ErrorMessage = "ContractNumber cannot exceed 100 characters")]
        public string ContractNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "ContractType is required")]
        public ContractType ContractType { get; set; }

        [Required(ErrorMessage = "StartDate is required")]
        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        [Required(ErrorMessage = "SignDate is required")]
        public DateOnly SignDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "ContractSalary must be non-negative")]
        public decimal ContractSalary { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "InsuranceSalary must be non-negative")]
        public decimal InsuranceSalary { get; set; }

        public string? AttachmentUrl { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.Active;
        public string? Notes { get; set; }
    }
}
