using System;

namespace QLNSCore.Entities
{
    public class Contract : BaseEntity<Guid>
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public Employee Employee { get; set; } = null!;

        public string ContractNumber { get; set; } = string.Empty;
        public ContractType ContractType { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateOnly SignDate { get; set; }

        public decimal ContractSalary { get; set; }
        public decimal InsuranceSalary { get; set; }
        public string? AttachmentUrl { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.Active;
        public string? Notes { get; set; }
    }

    public enum ContractType
    {
        Probation = 1,       // Thử việc
        DefiniteTerm = 2,    // Xác định thời hạn (1 - 3 năm)
        IndefiniteTerm = 3,  // Không xác định thời hạn
        Internship = 4,      // Thực tập sinh / Học việc
        PartTime = 5         // Bán thời gian / Cộng tác viên
    }

    public enum ContractStatus
    {
        Active = 1,     // Đang hiệu lực
        Expired = 2,    // Đã hết hạn
        Terminated = 3  // Đã chấm dứt / Thanh lý
    }
}
