import type { BonusPenaltyType } from './enums';

export interface SalariesDto {
  id: string;
  employeeId: string;
  employeeName: string;
  basicSalary: number;
  actualWorkDays: number;
  paidLeaveDays: number;
  allowance: number;
  bonus: number;
  penalty: number;
  overtimePay: number;
  grossSalary: number;
  insurance: number;
  tax: number;
  totalSalary: number;
  month: number;
  year: number;
}

export interface AddSalariesDto {
  employeeId: string;
  allowance: number;
  bonus: number;
  penalty: number;
  actualWorkDays: number;
  paidLeaveDays: number;
  overtimePay: number;
  insurance: number;
  tax: number;
  month: number;
  year: number;
}

export interface BonusPenaltyDto {
  id: string;
  employeeId: string;
  employeeName: string;
  type: BonusPenaltyType;
  amount: number;
  reason: string;
  date: string;
  month: number;
  year: number;
  createdDate?: string;
}

export interface AddBonusPenaltyDto {
  employeeId: string;
  type: BonusPenaltyType;
  amount: number;
  reason: string;
  date: string;
  month: number;
  year: number;
}

export interface PayrollDto {
  employeeId: string;
  employeeName: string;
  departmentName: string;
  month: number;
  year: number;
  basicSalary: number;
  standardWorkingDays: number;
  actualWorkDays: number;
  paidLeaveDays: number;
  actualBasicSalary: number;
  allowance: number;
  overtimeHours: number;
  overtimePay: number;
  totalBonus: number;
  totalPenalty: number;
  grossSalary: number;
  insurance: number;
  taxableIncome: number;
  tax: number;
  netSalary: number;
  workedDays: number;
  totalDays: number;
}

export interface PayrollGenerateDto {
  month: number;
  year: number;
  calculateAttendance?: boolean;
  standardWorkingDays?: number;
  includeInsurance?: boolean;
  includeTax?: boolean;
  saveToDatabase?: boolean;
}
