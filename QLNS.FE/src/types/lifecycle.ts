import type { EmployeeStatus } from './enums';

export interface LifecycleStageDurationDto {
  stageKey: 'Internship' | 'Probation' | 'Official' | string;
  hasCompletedOrActive: boolean;
  isCurrent: boolean;
  startDate?: string | null;
  endDate?: string | null;
  totalMonths: number;
  totalDays: number;
  contractCount: number;
  representativeSalary: number;
}

export interface MonthlyLifecycleMetricDto {
  month: number;
  year: number;
  label: string;
  cumulativeTenureMonths: number;
  regularHours: number;
  overtimeHours: number;
  punctualityRate: number;
  netSalary: number;
  bonusAmount: number;
  penaltyAmount: number;
}

export interface LifecycleMilestoneDto {
  date: string;
  category: 'Onboarding' | 'Internship' | 'Probation' | 'Official' | 'Bonus' | 'Penalty' | string;
  codeOrRef: string;
  description: string;
  amount?: number | null;
}

export interface EmployeeLifecycleSummaryDto {
  employeeId: string;
  code: string;
  fullName: string;
  departmentName: string;
  positionName: string;
  faceImageUrl?: string | null;
  joinDate: string;
  status: EmployeeStatus;
  currentStage: 'Internship' | 'Probation' | 'Official' | 'Resigned' | string;
  internshipMonths: number;
  probationMonths: number;
  officialMonths: number;
  totalTenureMonths: number;
  currentSalary: number;
  monthlyRegularHours: number;
  monthlyOvertimeHours: number;
}

export interface CompanyLifecycleOverviewDto {
  asOfMonth: number;
  asOfYear: number;
  totalEmployees: number;
  internshipCount: number;
  probationCount: number;
  officialCount: number;
  resignedCount: number;
  averageTenureMonths: number;
  retentionRate: number;
  employees: EmployeeLifecycleSummaryDto[];
}

export interface EmployeeLifecycleDetailDto {
  employeeId: string;
  code: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  departmentName: string;
  positionName: string;
  managerName?: string | null;
  faceImageUrl?: string | null;
  joinDate: string;
  status: EmployeeStatus;
  currentStage: 'Internship' | 'Probation' | 'Official' | 'Resigned' | string;
  asOfMonth: number;
  asOfYear: number;

  internshipStage: LifecycleStageDurationDto;
  probationStage: LifecycleStageDurationDto;
  officialStage: LifecycleStageDurationDto;

  totalTenureMonths: number;
  totalTenureDays: number;
  initialSalary: number;
  currentSalary: number;
  salaryGrowthPercent: number;
  currentMonthRegularHours: number;
  currentMonthOvertimeHours: number;
  currentMonthWorkDays: number;
  overallPunctualityRate: number;
  onTimeDays: number;
  lateDays: number;
  leaveDaysUsed: number;
  leaveDaysRemaining: number;
  totalBonusCount: number;
  totalBonusAmount: number;
  totalPenaltyCount: number;
  totalPenaltyAmount: number;
  totalContractsCount: number;

  monthlySeries: MonthlyLifecycleMetricDto[];
  milestones: LifecycleMilestoneDto[];
}
