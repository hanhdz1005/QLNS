import apiClient from './client';
import type { ApiResponse, CompanyLifecycleOverviewDto, EmployeeLifecycleDetailDto } from '@/types';

export const lifecycleApi = {
  getCompanyOverview: async (month?: number, year?: number) => {
    const { data } = await apiClient.get<ApiResponse<CompanyLifecycleOverviewDto>>('/Lifecycle/overview', {
      params: { month, year },
    });
    return data;
  },

  getOverview: async (month?: number, year?: number) => {
    const { data } = await apiClient.get<ApiResponse<CompanyLifecycleOverviewDto>>('/Lifecycle/overview', {
      params: { month, year },
    });
    return data;
  },

  getMyLifecycle: async (month?: number, year?: number) => {
    const { data } = await apiClient.get<ApiResponse<EmployeeLifecycleDetailDto>>('/Lifecycle/me', {
      params: { month, year },
    });
    return data;
  },

  getEmployeeLifecycle: async (employeeId: string, month?: number, year?: number) => {
    const { data } = await apiClient.get<ApiResponse<EmployeeLifecycleDetailDto>>(`/Lifecycle/${employeeId}`, {
      params: { month, year },
    });
    return data;
  },
};
