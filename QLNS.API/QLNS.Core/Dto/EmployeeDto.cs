using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Dto
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Guid PositionId { get; set; }
        public string PositionName { get; set; }
        public DateOnly JoinDate { get; set; }
        public EmployeeStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string? CitizenId { get; set; }
        public DateOnly? CitizenIdIssueDate { get; set; }
        public string? CitizenIdIssuePlace { get; set; }
        public string? TaxCode { get; set; }
        public string? SocialInsuranceNumber { get; set; }
        public int DependentsCount { get; set; }

        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankBranch { get; set; }

        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactRelationship { get; set; }

        public Guid? ManagerId { get; set; }
        public string? ManagerName { get; set; }
    }

    public class AddEmployeeDto
    {

        [Required(ErrorMessage = "FullName is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "FullName must be between 2 and 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "DateOfBirth is required")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "PhoneNumber cannot exceed 20 characters")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "DepartmentId is required")]
        public Guid DepartmentId { get; set; }

        [Required(ErrorMessage = "PositionId is required")]
        public Guid PositionId { get; set; }

        [Required(ErrorMessage = "JoinDate is required")]
        public DateOnly JoinDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public EmployeeStatus Status { get; set; }

        public string? CitizenId { get; set; }
        public DateOnly? CitizenIdIssueDate { get; set; }
        public string? CitizenIdIssuePlace { get; set; }
        public string? TaxCode { get; set; }
        public string? SocialInsuranceNumber { get; set; }
        public int DependentsCount { get; set; } = 0;

        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankBranch { get; set; }

        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactRelationship { get; set; }

        public Guid? ManagerId { get; set; }
    }
}