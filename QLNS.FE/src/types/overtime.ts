export enum OvertimeStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Cancelled = 'Cancelled',
}

export { OvertimeStatus as OvertimeRequestStatus };

export interface OvertimeRequestDto {
  id: string;
  employeeId: string;
  employeeCode?: string;
  employeeName: string;
  departmentName: string;
  date: string;
  registeredHours: number;
  reason: string;
  status: OvertimeStatus | number | string;
  approvedBy?: string | null;
  approvedByName?: string | null;
  createdDate: string;
}

export interface AddOvertimeRequestDto {
  employeeId?: string;
  date: string;
  registeredHours: number;
  reason: string;
}

export type CreateOvertimeRequestDto = AddOvertimeRequestDto;
