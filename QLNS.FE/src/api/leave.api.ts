import apiClient from './client';
import type {
  ApiResponse,
  PagedResult,
  ListItems,
  LeaveRequestDto,
  AddLeaveRequestDto,
  LeaveBalanceDto,
  UpdateLeaveBalanceDto,
} from '@/types';

export const leaveApi = {
  // Requests
  getAllRequests: async (): Promise<ApiResponse<ListItems<LeaveRequestDto>>> => {
    const res = await apiClient.get<ApiResponse<ListItems<LeaveRequestDto>>>('/LeaveRequest');
    return res.data;
  },

  getFilterRequests: async (
    pageIndex = 1,
    pageSize = 10,
    search?: string,
    myOnly = false
  ): Promise<ApiResponse<PagedResult<LeaveRequestDto>>> => {
    const params: Record<string, unknown> = { pageIndex, pageSize, myOnly };
    if (search) params.search = search;
    const res = await apiClient.get<ApiResponse<PagedResult<LeaveRequestDto>>>('/LeaveRequest/filter', { params });
    return res.data;
  },

  getRequestById: async (id: string): Promise<ApiResponse<LeaveRequestDto>> => {
    const res = await apiClient.get<ApiResponse<LeaveRequestDto>>(`/LeaveRequest/${id}`);
    return res.data;
  },

  createRequest: async (dto: AddLeaveRequestDto): Promise<ApiResponse<LeaveRequestDto>> => {
    const res = await apiClient.post<ApiResponse<LeaveRequestDto>>('/LeaveRequest', dto);
    return res.data;
  },

  updateRequest: async (id: string, dto: AddLeaveRequestDto): Promise<ApiResponse<LeaveRequestDto>> => {
    const res = await apiClient.put<ApiResponse<LeaveRequestDto>>(`/LeaveRequest/${id}`, dto);
    return res.data;
  },

  deleteRequest: async (id: string): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.delete<ApiResponse<boolean>>(`/LeaveRequest/${id}`);
    return res.data;
  },

  // Approvals (Admin/Leader)
  getPendingApprovals: async (pageIndex = 1, pageSize = 10): Promise<ApiResponse<PagedResult<LeaveRequestDto>>> => {
    const res = await apiClient.get<ApiResponse<PagedResult<LeaveRequestDto>>>('/LeaveApproval/pending', {
      params: { pageIndex, pageSize },
    });
    return res.data;
  },

  approveRequest: async (id: string): Promise<ApiResponse<LeaveRequestDto>> => {
    const res = await apiClient.post<ApiResponse<LeaveRequestDto>>(`/LeaveApproval/${id}/approve`);
    return res.data;
  },

  rejectRequest: async (id: string): Promise<ApiResponse<LeaveRequestDto>> => {
    const res = await apiClient.post<ApiResponse<LeaveRequestDto>>(`/LeaveApproval/${id}/reject`);
    return res.data;
  },

  // Balance
  getMyBalance: async (year?: number): Promise<ApiResponse<LeaveBalanceDto>> => {
    const params: Record<string, unknown> = {};
    if (year) params.year = year;
    const res = await apiClient.get<ApiResponse<LeaveBalanceDto>>('/LeaveBalance/my-balance', { params });
    return res.data;
  },

  getBalanceByEmployeeId: async (employeeId: string, year?: number): Promise<ApiResponse<LeaveBalanceDto>> => {
    const params: Record<string, unknown> = {};
    if (year) params.year = year;
    const res = await apiClient.get<ApiResponse<LeaveBalanceDto>>(`/LeaveBalance/employee/${employeeId}`, { params });
    return res.data;
  },

  updateTotalDays: async (dto: UpdateLeaveBalanceDto): Promise<ApiResponse<LeaveBalanceDto>> => {
    const res = await apiClient.put<ApiResponse<LeaveBalanceDto>>('/LeaveBalance/update-total', dto);
    return res.data;
  },
};
