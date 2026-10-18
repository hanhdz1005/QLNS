import apiClient from './client';
import type {
  ApiResponse,
  PagedResult,
  ListItems,
  DepartmentDto,
  AddDepartmentDto,
} from '@/types';

export const departmentApi = {
  getAll: async (): Promise<ApiResponse<ListItems<DepartmentDto>>> => {
    const res = await apiClient.get<ApiResponse<ListItems<DepartmentDto>>>('/Department');
    return res.data;
  },

  getFilter: async (pageIndex = 1, pageSize = 10, search?: string): Promise<ApiResponse<PagedResult<DepartmentDto>>> => {
    const params: Record<string, unknown> = { pageIndex, pageSize };
    if (search) params.search = search;
    const res = await apiClient.get<ApiResponse<PagedResult<DepartmentDto>>>('/Department/filter', { params });
    return res.data;
  },

  getById: async (id: string): Promise<ApiResponse<DepartmentDto>> => {
    const res = await apiClient.get<ApiResponse<DepartmentDto>>(`/Department/${id}`);
    return res.data;
  },

  add: async (dto: AddDepartmentDto): Promise<ApiResponse<DepartmentDto>> => {
    const res = await apiClient.post<ApiResponse<DepartmentDto>>('/Department', dto);
    return res.data;
  },

  update: async (id: string, dto: AddDepartmentDto): Promise<ApiResponse<DepartmentDto>> => {
    const res = await apiClient.put<ApiResponse<DepartmentDto>>(`/Department/${id}`, dto);
    return res.data;
  },

  delete: async (id: string): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.delete<ApiResponse<boolean>>(`/Department/${id}`);
    return res.data;
  },
};
