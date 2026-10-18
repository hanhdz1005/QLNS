import type { Gender, EmployeeStatus } from './enums';

export interface EmployeeDto {
  id: string;
  code: string;
  fullName: string;
  dateOfBirth: string;
  gender: Gender;
  phoneNumber: string;
  email: string;
  address: string;
  departmentId: string;
  departmentName: string;
  positionId: string;
  positionName: string;
  joinDate: string;
  status: EmployeeStatus;
  createDate?: string;
  updatedDate?: string;

  citizenId?: string;
  citizenIdIssueDate?: string;
  citizenIdIssuePlace?: string;
  taxCode?: string;
  socialInsuranceNumber?: string;
  dependentsCount: number;

  bankAccountNumber?: string;
  bankName?: string;
  bankBranch?: string;

  emergencyContactName?: string;
  emergencyContactPhone?: string;
  emergencyContactRelationship?: string;

  managerId?: string;
  managerName?: string;
}

export interface AddEmployeeDto {
  fullName: string;
  dateOfBirth: string;
  gender: Gender;
  phoneNumber: string;
  email: string;
  address: string;
  departmentId: string;
  positionId: string;
  joinDate: string;
  status: EmployeeStatus;

  citizenId?: string;
  citizenIdIssueDate?: string;
  citizenIdIssuePlace?: string;
  taxCode?: string;
  socialInsuranceNumber?: string;
  dependentsCount?: number;

  bankAccountNumber?: string;
  bankName?: string;
  bankBranch?: string;

  emergencyContactName?: string;
  emergencyContactPhone?: string;
  emergencyContactRelationship?: string;

  managerId?: string;
}

export interface DepartmentDto {
  id: string;
  departmentName: string;
  code?: string;
  description?: string;
  createDate?: string;
}

export interface AddDepartmentDto {
  departmentName: string;
  description?: string;
}

export interface PositionDto {
  id: string;
  positionName: string;
  baseSalary: number;
  description?: string;
}

export interface AddPositionDto {
  positionName: string;
  baseSalary: number;
  description?: string;
}
