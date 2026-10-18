import apiClient from './client';
import type {
  ApiResponse,
  PagedResult,
  ListItems,
  PositionDto,
  AddPositionDto,
} from '@/types';

export const positionApi = {
  getAll: async (): Promise<ApiResponse<ListItems<PositionDto>>> => {
    const res = await apiClient.get<ApiResponse<ListItems<PositionDto>>>('/Position');
    return res.data;
  },

  getFilter: async (pageIndex = 1, pageSize = 10, search?: string): Promise<ApiResponse<PagedResult<PositionDto>>> => {
    const params: Record<string, unknown> = { pageIndex, pageSize };
    if (search) params.search = search;
    const res = await apiClient.get<ApiResponse<PagedResult<PositionDto>>>('/Position/filter', { params });
    return res.data;
  },

  getById: async (id: string): Promise<ApiResponse<PositionDto>> => {
    const res = await apiClient.get<ApiResponse<PositionDto>>(`/Position/${id}`);
    return res.data;
  },

  add: async (dto: AddPositionDto): Promise<ApiResponse<PositionDto>> => {
    const res = await apiClient.post<ApiResponse<PositionDto>>('/Position', dto);
    return res.data;
  },

  update: async (id: string, dto: AddPositionDto): Promise<ApiResponse<PositionDto>> => {
    const res = await apiClient.put<ApiResponse<PositionDto>>(`/Position/${id}`, dto);
    return res.data;
  },

  delete: async (id: string): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.delete<ApiResponse<boolean>>(`/Position/${id}`);
    return res.data;
  },
};
