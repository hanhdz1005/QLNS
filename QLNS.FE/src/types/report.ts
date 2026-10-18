export interface DashboardDto {
  totalEmployees: number;
  activeEmployees: number;
  totalDepartments: number;
  pendingLeaveRequests: number;
  totalAttendancesThisMonth: number;
  totalSalaryLastMonth: number;
  averageSalary: number;
}

export interface EmployeeStatsDto {
  departmentId: string;
  departmentName: string;
  employeeCount: number;
}

export interface LeaveReportDto {
  totalRequests: number;
  pending: number;
  approved: number;
  rejected: number;
  cancelled: number;
}

export interface SalaryReportDto {
  totalRecords: number;
  totalBasicSalary: number;
  totalNetSalary: number;
  averageNetSalary: number;
  maxSalary: number;
  minSalary: number;
}

export interface ExpiringContractItemDto {
  contractId: string;
  employeeId: string;
  employeeName: string;
  contractNumber: string;
  endDate?: string;
  daysRemaining: number;
}

export interface BirthdayItemDto {
  employeeId: string;
  employeeName: string;
  departmentName: string;
  dateOfBirth: string;
  day: number;
}

export interface ProbationEndingItemDto {
  employeeId: string;
  employeeName: string;
  departmentName: string;
  joinDate: string;
  estimatedProbationEnd: string;
  daysRemaining: number;
}

export interface HrAlertsDto {
  expiringContracts: ExpiringContractItemDto[];
  upcomingBirthdays: BirthdayItemDto[];
  probationEndingEmployees: ProbationEndingItemDto[];
}
