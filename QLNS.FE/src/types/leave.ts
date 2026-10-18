import type { LeaveType, LeaveStatus } from './enums';

export interface LeaveRequestDto {
  id: string;
  employeeId: string;
  employeeName: string;
  fromDate: string;
  toDate: string;
  reason: string;
  leaveType: LeaveType;
  status: LeaveStatus;
  aprovedBy?: string;
  createdDate?: string;
}

export interface AddLeaveRequestDto {
  employeeId: string;
  fromDate: string;
  toDate: string;
  reason: string;
  leaveType: LeaveType;
}

export interface LeaveBalanceDto {
  id: string;
  employeeId: string;
  employeeName: string;
  year: number;
  totalLeaveDays: number;
  usedLeaveDays: number;
  remainingLeaveDays: number;
}

export interface UpdateLeaveBalanceDto {
  employeeId: string;
  year: number;
  totalLeaveDays: number;
}
