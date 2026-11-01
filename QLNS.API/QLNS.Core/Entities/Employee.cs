using Microsoft.EntityFrameworkCore;
using QLNSCore.Entities;
using System.Collections.Generic;

namespace QLNSCore.Entities
{
    public class Employee : BaseEntity<Guid>
    {
        public string Code { get; set; }
        public string FullName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Department Department { get; set; }
        public Guid PositionId { get; set; }
        public string PositionName { get; set; }
        public Position Position { get; set; }
        public DateOnly JoinDate { get; set; }
        public EmployeeStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Thông tin định danh & thuế
        public string? CitizenId { get; set; }
        public DateOnly? CitizenIdIssueDate { get; set; }
        public string? CitizenIdIssuePlace { get; set; }
        public string? TaxCode { get; set; }
        public string? SocialInsuranceNumber { get; set; }
        public int DependentsCount { get; set; } = 0; // Người phụ thuộc giảm trừ gia cảnh

        // Thông tin tài khoản ngân hàng chi trả lương
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankBranch { get; set; }

        // Liên hệ khẩn cấp
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactRelationship { get; set; }

        // Phân cấp quản lý trực tiếp (Manager / Org Chart)
        public Guid? ManagerId { get; set; }
        public Employee? Manager { get; set; }
        public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();

        // Dữ liệu nhận diện khuôn mặt (Face Recognition)
        public string? FaceEmbedding { get; set; }
        public string? FaceImageUrl { get; set; }
        public DateTime? FaceRegisteredAt { get; set; }

        // Navigation collections
        public ICollection<Salary> Salaries { get; set; } = new List<Salary>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public ICollection<BonusPenalty> BonusPenalties { get; set; } = new List<BonusPenalty>();
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
        public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
    }

    public enum EmployeeStatus
    {
        Active = 1,
        Inactive = 2,
        Resigned = 3,
        Probation = 4
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }
}
