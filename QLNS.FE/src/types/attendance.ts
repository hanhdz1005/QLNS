import type { AttendanceStatus } from './enums';

export interface AttendanceDto {
  id: string;
  employeeId: string;
  employeeName: string;
  date: string;
  checkIn?: string;
  checkOut?: string;
  hoursWorked?: number;
  status: AttendanceStatus;
}

export interface AddAttendanceDto {
  employeeId: string;
  date: string;
  checkIn?: string;
  checkOut?: string;
  status: AttendanceStatus;
}

export interface QrTokenDto {
  token: string;
  expiresInSeconds: number;
  generatedAt: string;
}

export interface QrCheckInResultDto {
  success: boolean;
  actionType: string;
  message: string;
  employeeName: string;
  date: string;
  time: string;
  hoursWorked?: number;
}
