import apiClient from './client';
import type { ApiResponse, ListItems, OvertimeRequestDto, AddOvertimeRequestDto } from '@/types';

export const overtimeApi = {
  getMyRequests: async () => {
    const { data } = await apiClient.get<ApiResponse<ListItems<OvertimeRequestDto>>>('/Overtime/my');
    return data;
  },

  getAllRequests: async () => {
    const { data } = await apiClient.get<ApiResponse<ListItems<OvertimeRequestDto>>>('/Overtime');
    return data;
  },

  getPendingApprovals: async () => {
    const { data } = await apiClient.get<ApiResponse<ListItems<OvertimeRequestDto>>>('/Overtime/pending');
    return data;
  },

  getPendingRequests: async () => {
    const { data } = await apiClient.get<ApiResponse<ListItems<OvertimeRequestDto>>>('/Overtime/pending');
    return data;
  },

  createRequest: async (dto: AddOvertimeRequestDto) => {
    const { data } = await apiClient.post<ApiResponse<OvertimeRequestDto>>('/Overtime', dto);
    return data;
  },

  approveRequest: async (id: string) => {
    const { data } = await apiClient.post<ApiResponse<OvertimeRequestDto>>(`/Overtime/${id}/approve`);
    return data;
  },

  rejectRequest: async (id: string) => {
    const { data } = await apiClient.post<ApiResponse<OvertimeRequestDto>>(`/Overtime/${id}/reject`);
    return data;
  },
};
