export enum OvertimeStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Cancelled = 'Cancelled',
}

export { OvertimeStatus as OvertimeRequestStatus };

export enum OvertimeType {
  Weekday = 'Weekday',
  Weekend = 'Weekend',
  Holiday = 'Holiday',
}

export enum OvertimeCompensationType {
  CompensatoryLeave = 'CompensatoryLeave',
  RecognitionOnly = 'RecognitionOnly',
  Paid = 'Paid',
}

export interface OvertimeRequestDto {
  id: string;
  employeeId: string;
  employeeCode?: string;
  employeeName: string;
  departmentName: string;
  date: string;
  startTime?: string | null;
  endTime?: string | null;
  registeredHours: number;
  otType: OvertimeType | number | string;
  multiplier: number;
  compensationType: OvertimeCompensationType | number | string;
  reason: string;
  status: OvertimeStatus | number | string;
  approvedBy?: string | null;
  approvedByName?: string | null;
  approverNote?: string | null;
  createdDate: string;
  actualCheckIn?: string | null;
  actualCheckOut?: string | null;
  actualHoursWorked?: number | null;
}

export interface AddOvertimeRequestDto {
  employeeId?: string;
  date: string;
  startTime?: string | null;
  endTime?: string | null;
  registeredHours: number;
  otType?: OvertimeType | string | number;
  compensationType?: OvertimeCompensationType | string | number;
  reason: string;
}

export type CreateOvertimeRequestDto = AddOvertimeRequestDto;

export interface ReviewOvertimeRequestDto {
  approvedHours?: number;
  compensationType?: OvertimeCompensationType | string | number;
  otType?: OvertimeType | string | number;
  approverNote?: string;
}

export interface OvertimeMonthlySummaryDto {
  month: number;
  year: number;
  totalApprovedHours: number;
  monthlyLimitHours: number;
  compensatoryHours: number;
  compensatoryDaysAdded: number;
  recognitionHours: number;
  paidHours: number;
  pendingCount: number;
}
