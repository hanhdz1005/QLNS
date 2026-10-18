import type { ContractType, ContractStatus } from './enums';

export interface ContractDto {
  id: string;
  employeeId: string;
  employeeName: string;
  contractNumber: string;
  contractType: ContractType;
  startDate: string;
  endDate?: string;
  signDate: string;
  contractSalary: number;
  insuranceSalary: number;
  attachmentUrl?: string;
  status: ContractStatus;
  notes?: string;
}

export interface AddContractDto {
  employeeId: string;
  contractNumber: string;
  contractType: ContractType;
  startDate: string;
  endDate?: string;
  signDate: string;
  contractSalary: number;
  insuranceSalary: number;
  attachmentUrl?: string;
  status: ContractStatus;
  notes?: string;
}
