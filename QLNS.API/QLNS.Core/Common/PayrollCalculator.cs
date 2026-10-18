using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QLNS.Core.Common
{
    public class PayrollCalculationResult
    {
        public decimal BaseSalary { get; set; }
        public int StandardWorkingDays { get; set; }
        public decimal ActualWorkDays { get; set; }
        public decimal PaidLeaveDays { get; set; }
        public decimal TotalPaidDays { get; set; }
        public decimal ActualBasicSalary { get; set; }
        public decimal Allowance { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalPenalty { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal Insurance { get; set; } // BHXH 8% + BHYT 1.5% + BHTN 1% = 10.5%
        public decimal TaxableIncome { get; set; }
        public decimal PersonalIncomeTax { get; set; }
        public decimal NetSalary { get; set; }
    }

    public static class PayrollCalculator
    {
        // Giảm trừ gia cảnh bản thân theo Luật Thuế TNCN Việt Nam hiện hành: 11,000,000 VND / tháng
        public const decimal PersonalRelief = 11000000m;
        // Giảm trừ gia cảnh cho mỗi người phụ thuộc: 4,400,000 VND / tháng
        public const decimal DependentRelief = 4400000m;

        // Tỷ lệ trích đóng bảo hiểm của người lao động: BHXH 8%, BHYT 1.5%, BHTN 1% => 10.5%
        public const decimal EmployeeSocialInsuranceRate = 0.08m;
        public const decimal EmployeeHealthInsuranceRate = 0.015m;
        public const decimal EmployeeUnemploymentInsuranceRate = 0.01m;
        public const decimal TotalInsuranceRate = EmployeeSocialInsuranceRate + EmployeeHealthInsuranceRate + EmployeeUnemploymentInsuranceRate; // 10.5%

        /// <summary>
        /// Tính số ngày làm việc chuẩn trong tháng (mặc định loại trừ các ngày Chủ nhật, tương đương làm Thứ 2 - Thứ 7, ~26 ngày).
        /// Nếu excludeSaturday = true thì loại trừ cả Thứ 7 và Chủ nhật (~22 ngày công chuẩn).
        /// </summary>
        public static int CalculateStandardWorkingDays(int year, int month, bool excludeSaturday = false)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var workingDays = 0;

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);
                if (date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                if (excludeSaturday && date.DayOfWeek == DayOfWeek.Saturday)
                    continue;

                workingDays++;
            }

            return workingDays > 0 ? workingDays : 26;
        }

        /// <summary>
        /// Tính số ngày nghỉ phép có lương (Approved Leave) trong tháng (loại trừ Chủ nhật).
        /// </summary>
        public static decimal CalculatePaidLeaveDays(IEnumerable<LeaveRequest> approvedLeaves, int year, int month)
        {
            var paidDays = 0m;
            var monthStart = new DateOnly(year, month, 1);
            var monthEnd = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            foreach (var leave in approvedLeaves)
            {
                var from = leave.FromDate < monthStart ? monthStart : leave.FromDate;
                var to = leave.ToDate > monthEnd ? monthEnd : leave.ToDate;

                if (from > to) continue;

                var curr = from;
                while (curr <= to)
                {
                    var dt = curr.ToDateTime(TimeOnly.MinValue);
                    if (dt.DayOfWeek != DayOfWeek.Sunday)
                    {
                        paidDays += 1.0m;
                    }
                    curr = curr.AddDays(1);
                }
            }

            return paidDays;
        }

        /// <summary>
        /// Phân tích dữ liệu chấm công: số ngày công thực tế, giờ OT và tiền OT (150%).
        /// </summary>
        public static (decimal ActualWorkDays, decimal OtHours, decimal OtPay) CalculateAttendanceDetails(
            IEnumerable<Attendance> attendances,
            decimal baseSalary,
            int standardWorkingDays)
        {
            var actualWorkDays = 0m;
            var totalOtHours = 0m;

            var hourlyRate = standardWorkingDays > 0
                ? baseSalary / (standardWorkingDays * 8.0m)
                : 0m;

            foreach (var att in attendances)
            {
                switch (att.Status)
                {
                    case AttendanceStatus.Present:
                    case AttendanceStatus.WorkFromHome:
                        if (att.HoursWorked.HasValue && att.HoursWorked.Value > 0)
                        {
                            if (att.HoursWorked.Value >= 7.5m)
                            {
                                actualWorkDays += 1.0m;
                                if (att.HoursWorked.Value > 8.0m)
                                {
                                    totalOtHours += (att.HoursWorked.Value - 8.0m);
                                }
                            }
                            else if (att.HoursWorked.Value >= 4.0m)
                            {
                                actualWorkDays += 0.5m;
                            }
                        }
                        else
                        {
                            actualWorkDays += 1.0m;
                        }
                        break;

                    case AttendanceStatus.Late:
                    case AttendanceStatus.EarlyLeave:
                        if (att.HoursWorked.HasValue && att.HoursWorked.Value > 0)
                        {
                            if (att.HoursWorked.Value >= 7.0m)
                                actualWorkDays += 1.0m;
                            else if (att.HoursWorked.Value >= 4.0m)
                                actualWorkDays += 0.5m;
                        }
                        else
                        {
                            actualWorkDays += 1.0m;
                        }
                        break;

                    case AttendanceStatus.Absent:
                    case AttendanceStatus.Leave:
                    default:
                        break;
                }
            }

            // Tiền làm thêm giờ = Số giờ OT * Đơn giá giờ * Hệ số 1.5 (ngày thường)
            var otPay = Math.Round(totalOtHours * hourlyRate * 1.5m, 0);

            return (actualWorkDays, totalOtHours, otPay);
        }

        /// <summary>
        /// Tính tiền bảo hiểm bắt buộc người lao động phải nộp (10.5%).
        /// </summary>
        public static decimal CalculateInsurance(decimal baseSalary, bool includeInsurance = true)
        {
            if (!includeInsurance || baseSalary <= 0)
                return 0m;

            return Math.Round(baseSalary * TotalInsuranceRate, 0);
        }

        /// <summary>
        /// Tính thuế thu nhập cá nhân (PIT) theo biểu thuế lũy tiến từng phần 7 bậc của Việt Nam.
        /// </summary>
        public static (decimal TaxableIncome, decimal Tax) CalculatePersonalIncomeTax(
            decimal grossSalary,
            decimal insurance,
            decimal nonTaxableAllowance = 0m,
            decimal nonTaxableOtPay = 0m,
            decimal dependentCount = 0,
            bool includeTax = true)
        {
            if (!includeTax)
                return (0m, 0m);

            // Thu nhập chịu thuế = Tổng thu nhập Gross - Các khoản miễn thuế (Ăn trưa, phần OT vượt chuẩn)
            var incomeSubjectToTax = Math.Max(0m, grossSalary - nonTaxableAllowance - nonTaxableOtPay);

            // Các khoản giảm trừ: Giảm trừ gia cảnh bản thân (11tr) + người phụ thuộc (4.4tr/người) + Bảo hiểm bắt buộc
            var totalDeductions = PersonalRelief + (dependentCount * DependentRelief) + insurance;

            // Thu nhập tính thuế (TNTT)
            var assessableIncome = Math.Max(0m, incomeSubjectToTax - totalDeductions);

            if (assessableIncome <= 0)
                return (0m, 0m);

            decimal tax = 0m;

            // Biểu thuế lũy tiến từng phần (VNĐ/tháng):
            // Bậc 1: Đến 5 triệu: 5%
            // Bậc 2: Trên 5 tr đến 10 tr: 10% - 250,000
            // Bậc 3: Trên 10 tr đến 18 tr: 15% - 750,000
            // Bậc 4: Trên 18 tr đến 32 tr: 20% - 1,650,000
            // Bậc 5: Trên 32 tr đến 52 tr: 25% - 3,250,000
            // Bậc 6: Trên 52 tr đến 80 tr: 30% - 5,850,000
            // Bậc 7: Trên 80 triệu: 35% - 9,850,000
            if (assessableIncome <= 5000000m)
            {
                tax = assessableIncome * 0.05m;
            }
            else if (assessableIncome <= 10000000m)
            {
                tax = (assessableIncome * 0.10m) - 250000m;
            }
            else if (assessableIncome <= 18000000m)
            {
                tax = (assessableIncome * 0.15m) - 750000m;
            }
            else if (assessableIncome <= 32000000m)
            {
                tax = (assessableIncome * 0.20m) - 1650000m;
            }
            else if (assessableIncome <= 52000000m)
            {
                tax = (assessableIncome * 0.25m) - 3250000m;
            }
            else if (assessableIncome <= 80000000m)
            {
                tax = (assessableIncome * 0.30m) - 5850000m;
            }
            else
            {
                tax = (assessableIncome * 0.35m) - 9850000m;
            }

            return (assessableIncome, Math.Round(Math.Max(0m, tax), 0));
        }

        /// <summary>
        /// Tổng hợp toàn bộ bảng tính lương chuyên nghiệp cho một nhân viên.
        /// </summary>
        public static PayrollCalculationResult Calculate(
            decimal baseSalary,
            decimal allowance,
            int standardWorkingDays,
            decimal actualWorkDays,
            decimal paidLeaveDays,
            decimal overtimeHours,
            decimal overtimePay,
            decimal totalBonus,
            decimal totalPenalty,
            bool calculateAttendance = true,
            bool includeInsurance = true,
            bool includeTax = true)
        {
            var totalPaidDays = actualWorkDays + paidLeaveDays;

            decimal actualBasicSalary;
            if (calculateAttendance && standardWorkingDays > 0)
            {
                var ratio = Math.Min(1.0m, totalPaidDays / (decimal)standardWorkingDays);
                actualBasicSalary = Math.Round(baseSalary * ratio, 0);
            }
            else
            {
                actualBasicSalary = baseSalary;
            }

            // Tổng thu nhập trước thuế (Gross) = Lương cơ bản theo công + Phụ cấp + Thưởng + Tiền làm thêm giờ
            var grossSalary = actualBasicSalary + allowance + totalBonus + overtimePay;

            // Bảo hiểm 10.5% trên lương cơ bản
            var insurance = CalculateInsurance(baseSalary, includeInsurance);

            // Thuế TNCN
            // Phụ cấp ăn trưa miễn thuế tối đa 730,000 VND nếu có phụ cấp
            var nonTaxableAllowance = Math.Min(allowance, 730000m);
            // Phần chênh lệch lương làm thêm giờ (1.5x - 1.0x = 0.5x) được miễn thuế theo Luật Thuế TNCN
            var nonTaxableOt = overtimeHours > 0 ? Math.Round(overtimePay / 3.0m, 0) : 0m;

            var (taxableIncome, tax) = CalculatePersonalIncomeTax(
                grossSalary,
                insurance,
                nonTaxableAllowance,
                nonTaxableOt,
                dependentCount: 0,
                includeTax: includeTax);

            // Lương thực nhận (Net) = Gross - Phạt - Bảo hiểm - Thuế
            var netSalary = Math.Max(0m, grossSalary - totalPenalty - insurance - tax);

            return new PayrollCalculationResult
            {
                BaseSalary = baseSalary,
                StandardWorkingDays = standardWorkingDays,
                ActualWorkDays = actualWorkDays,
                PaidLeaveDays = paidLeaveDays,
                TotalPaidDays = totalPaidDays,
                ActualBasicSalary = actualBasicSalary,
                Allowance = allowance,
                OvertimeHours = overtimeHours,
                OvertimePay = overtimePay,
                TotalBonus = totalBonus,
                TotalPenalty = totalPenalty,
                GrossSalary = grossSalary,
                Insurance = insurance,
                TaxableIncome = taxableIncome,
                PersonalIncomeTax = tax,
                NetSalary = netSalary
            };
        }
    }
}
