import apiClient from './client';
import type {
  ApiResponse,
  PagedResult,
  ListItems,
  EmployeeDto,
  AddEmployeeDto,
} from '@/types';

export const employeeApi = {
  getAll: async (): Promise<ApiResponse<ListItems<EmployeeDto>>> => {
    const res = await apiClient.get<ApiResponse<ListItems<EmployeeDto>>>('/Employee');
    return res.data;
  },

  getFilter: async (pageIndex = 1, pageSize = 10, search?: string): Promise<ApiResponse<PagedResult<EmployeeDto>>> => {
    const params: Record<string, unknown> = { pageIndex, pageSize };
    if (search) params.search = search;
    const res = await apiClient.get<ApiResponse<PagedResult<EmployeeDto>>>('/Employee/filter', { params });
    return res.data;
  },

  getById: async (id: string): Promise<ApiResponse<EmployeeDto>> => {
    const res = await apiClient.get<ApiResponse<EmployeeDto>>(`/Employee/${id}`);
    return res.data;
  },

  getProfileMe: async (): Promise<ApiResponse<EmployeeDto>> => {
    const res = await apiClient.get<ApiResponse<EmployeeDto>>('/Employee/profile/me');
    return res.data;
  },

  add: async (dto: AddEmployeeDto): Promise<ApiResponse<EmployeeDto>> => {
    const res = await apiClient.post<ApiResponse<EmployeeDto>>('/Employee', dto);
    return res.data;
  },

  update: async (id: string, dto: AddEmployeeDto): Promise<ApiResponse<EmployeeDto>> => {
    const res = await apiClient.put<ApiResponse<EmployeeDto>>(`/Employee/${id}`, dto);
    return res.data;
  },

  delete: async (id: string): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.delete<ApiResponse<boolean>>(`/Employee/${id}`);
    return res.data;
  },
};
