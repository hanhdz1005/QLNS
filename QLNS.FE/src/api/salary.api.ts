import apiClient from './client';
import type {
  ApiResponse,
  PagedResult,
  ListItems,
  SalariesDto,
  AddSalariesDto,
  BonusPenaltyDto,
  AddBonusPenaltyDto,
  PayrollDto,
  PayrollGenerateDto,
} from '@/types';

export const salaryApi = {
  // Base Salary / Salaries
  getAllSalaries: async (): Promise<ApiResponse<ListItems<SalariesDto>>> => {
    const res = await apiClient.get<ApiResponse<ListItems<SalariesDto>>>('/Salary');
    return res.data;
  },

  getFilterSalaries: async (pageIndex = 1, pageSize = 10, search?: string): Promise<ApiResponse<PagedResult<SalariesDto>>> => {
    const params: Record<string, unknown> = { pageIndex, pageSize };
    if (search) params.search = search;
    const res = await apiClient.get<ApiResponse<PagedResult<SalariesDto>>>('/Salary/filter', { params });
    return res.data;
  },

  getSalaryById: async (id: string): Promise<ApiResponse<SalariesDto>> => {
    const res = await apiClient.get<ApiResponse<SalariesDto>>(`/Salary/${id}`);
    return res.data;
  },

  addSalary: async (dto: AddSalariesDto): Promise<ApiResponse<SalariesDto>> => {
    const res = await apiClient.post<ApiResponse<SalariesDto>>('/Salary', dto);
    return res.data;
  },

  updateSalary: async (id: string, dto: AddSalariesDto): Promise<ApiResponse<SalariesDto>> => {
    const res = await apiClient.put<ApiResponse<SalariesDto>>(`/Salary/${id}`, dto);
    return res.data;
  },

  deleteSalary: async (id: string): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.delete<ApiResponse<boolean>>(`/Salary/${id}`);
    return res.data;
  },

  // Bonus & Penalty
  getAllBonusPenalties: async (): Promise<ApiResponse<ListItems<BonusPenaltyDto>>> => {
    const res = await apiClient.get<ApiResponse<ListItems<BonusPenaltyDto>>>('/BonusPenalty');
    return res.data;
  },

  getFilterBonusPenalties: async (
    pageIndex = 1,
    pageSize = 10,
    search?: string,
    type?: number
  ): Promise<ApiResponse<PagedResult<BonusPenaltyDto>>> => {
    const params: Record<string, unknown> = { pageIndex, pageSize };
    if (search) params.search = search;
    if (type !== undefined && type !== null) params.type = type;
    const res = await apiClient.get<ApiResponse<PagedResult<BonusPenaltyDto>>>('/BonusPenalty/filter', { params });
    return res.data;
  },

  getBonusPenaltyById: async (id: string): Promise<ApiResponse<BonusPenaltyDto>> => {
    const res = await apiClient.get<ApiResponse<BonusPenaltyDto>>(`/BonusPenalty/${id}`);
    return res.data;
  },

  addBonusPenalty: async (dto: AddBonusPenaltyDto): Promise<ApiResponse<BonusPenaltyDto>> => {
    const res = await apiClient.post<ApiResponse<BonusPenaltyDto>>('/BonusPenalty', dto);
    return res.data;
  },

  updateBonusPenalty: async (id: string, dto: AddBonusPenaltyDto): Promise<ApiResponse<BonusPenaltyDto>> => {
    const res = await apiClient.put<ApiResponse<BonusPenaltyDto>>(`/BonusPenalty/${id}`, dto);
    return res.data;
  },

  deleteBonusPenalty: async (id: string): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.delete<ApiResponse<boolean>>(`/BonusPenalty/${id}`);
    return res.data;
  },

  // Payroll
  getAllPayroll: async (month: number, year: number): Promise<ApiResponse<ListItems<PayrollDto>>> => {
    const res = await apiClient.get<ApiResponse<ListItems<PayrollDto>>>('/Payroll', {
      params: { month, year },
    });
    return res.data;
  },

  generatePayroll: async (
    employeeId: string,
    month: number,
    year: number,
    saveToDb = true
  ): Promise<ApiResponse<PayrollDto>> => {
    const res = await apiClient.get<ApiResponse<PayrollDto>>('/Payroll/generate', {
      params: { employeeId, month, year, saveToDb },
    });
    return res.data;
  },

  generateBulkPayroll: async (dto: PayrollGenerateDto): Promise<ApiResponse<ListItems<PayrollDto>>> => {
    const res = await apiClient.post<ApiResponse<ListItems<PayrollDto>>>('/Payroll/generate-bulk', dto);
    return res.data;
  },

  getMyPayslip: async (month?: number, year?: number): Promise<ApiResponse<PayrollDto>> => {
    const params: Record<string, unknown> = {};
    if (month) params.month = month;
    if (year) params.year = year;
    const res = await apiClient.get<ApiResponse<PayrollDto>>('/Payroll/my-payslip', { params });
    return res.data;
  },
};
