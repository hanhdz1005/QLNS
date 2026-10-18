import apiClient from './client';
import type {
  ApiResponse,
  PagedResult,
  ListItems,
  ContractDto,
  AddContractDto,
} from '@/types';

export const contractApi = {
  getAll: async (): Promise<ApiResponse<ListItems<ContractDto>>> => {
    const res = await apiClient.get<ApiResponse<ListItems<ContractDto>>>('/Contract');
    return res.data;
  },

  getFilter: async (pageIndex = 1, pageSize = 10, search?: string): Promise<ApiResponse<PagedResult<ContractDto>>> => {
    const params: Record<string, unknown> = { pageIndex, pageSize };
    if (search) params.search = search;
    const res = await apiClient.get<ApiResponse<PagedResult<ContractDto>>>('/Contract/filter', { params });
    return res.data;
  },

  getById: async (id: string): Promise<ApiResponse<ContractDto>> => {
    const res = await apiClient.get<ApiResponse<ContractDto>>(`/Contract/${id}`);
    return res.data;
  },

  getByEmployeeId: async (employeeId: string): Promise<ApiResponse<ContractDto[]>> => {
    const res = await apiClient.get<ApiResponse<ContractDto[]>>(`/Contract/employee/${employeeId}`);
    return res.data;
  },

  getExpiring: async (days = 30): Promise<ApiResponse<ContractDto[]>> => {
    const res = await apiClient.get<ApiResponse<ContractDto[]>>('/Contract/expiring', { params: { days } });
    return res.data;
  },

  add: async (dto: AddContractDto): Promise<ApiResponse<ContractDto>> => {
    const res = await apiClient.post<ApiResponse<ContractDto>>('/Contract', dto);
    return res.data;
  },

  update: async (id: string, dto: AddContractDto): Promise<ApiResponse<ContractDto>> => {
    const res = await apiClient.put<ApiResponse<ContractDto>>(`/Contract/${id}`, dto);
    return res.data;
  },

  delete: async (id: string): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.delete<ApiResponse<boolean>>(`/Contract/${id}`);
    return res.data;
  },
};
