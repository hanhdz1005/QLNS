using QLNSCore.Entities;
using System;
using System.Collections.Generic;
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
        public string? FaceSnapshotUrl { get; set; }
        public decimal? SimilarityScore { get; set; }
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

    public class FaceRegisterDto
    {
        public string CapturedImageBase64 { get; set; } = string.Empty;
        public List<double> FaceDescriptor { get; set; } = new();
    }

    public class FaceCheckInDto
    {
        public string CapturedImageBase64 { get; set; } = string.Empty;
        public List<double> CapturedFaceDescriptor { get; set; } = new();
    }

    public class FaceCheckInResultDto
    {
        public bool Success { get; set; } = true;
        public string ActionType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public TimeSpan Time { get; set; }
        public decimal? HoursWorked { get; set; }
        public decimal? SimilarityScore { get; set; }
        public string? SnapshotUrl { get; set; }
    }

    public class CameraErrorReportDto
    {
        public string ErrorCode { get; set; } = "CAMERA_CONNECT_FAILED";
        public string? ErrorDetail { get; set; }
    }
}
