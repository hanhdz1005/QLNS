import apiClient from './client';
import type {
  ApiResponse,
  ListItems,
  OvertimeRequestDto,
  AddOvertimeRequestDto,
  ReviewOvertimeRequestDto,
  OvertimeMonthlySummaryDto,
} from '@/types';

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

  getMonthlySummary: async (month?: number, year?: number, companyWide = false) => {
    const { data } = await apiClient.get<ApiResponse<OvertimeMonthlySummaryDto>>('/Overtime/summary', {
      params: { month, year, companyWide },
    });
    return data;
  },

  createRequest: async (dto: AddOvertimeRequestDto) => {
    const { data } = await apiClient.post<ApiResponse<OvertimeRequestDto>>('/Overtime', dto);
    return data;
  },

  approveRequest: async (id: string, dto?: ReviewOvertimeRequestDto) => {
    const { data } = await apiClient.post<ApiResponse<OvertimeRequestDto>>(`/Overtime/${id}/approve`, dto || {});
    return data;
  },

  rejectRequest: async (id: string, dto?: ReviewOvertimeRequestDto) => {
    const { data } = await apiClient.post<ApiResponse<OvertimeRequestDto>>(`/Overtime/${id}/reject`, dto || {});
    return data;
  },

  cancelRequest: async (id: string) => {
    const { data } = await apiClient.post<ApiResponse<OvertimeRequestDto>>(`/Overtime/${id}/cancel`);
    return data;
  },
};
