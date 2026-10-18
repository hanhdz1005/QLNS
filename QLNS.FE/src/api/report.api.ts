import apiClient from './client';
import type {
  ApiResponse,
  ListItems,
  DashboardDto,
  EmployeeStatsDto,
  LeaveReportDto,
  SalaryReportDto,
  HrAlertsDto,
} from '@/types';

export const reportApi = {
  getDashboard: async (): Promise<ApiResponse<DashboardDto>> => {
    const res = await apiClient.get<ApiResponse<DashboardDto>>('/Report/dashboard');
    return res.data;
  },

  getEmployeeStats: async (): Promise<ApiResponse<ListItems<EmployeeStatsDto>>> => {
    const res = await apiClient.get<ApiResponse<ListItems<EmployeeStatsDto>>>('/Report/employee-stats');
    return res.data;
  },

  getLeaveReport: async (fromDate: string, toDate: string): Promise<ApiResponse<LeaveReportDto>> => {
    const res = await apiClient.get<ApiResponse<LeaveReportDto>>('/Report/leave-report', {
      params: { fromDate, toDate },
    });
    return res.data;
  },

  getSalaryReport: async (month: number, year: number): Promise<ApiResponse<SalaryReportDto>> => {
    const res = await apiClient.get<ApiResponse<SalaryReportDto>>('/Report/salary-report', {
      params: { month, year },
    });
    return res.data;
  },

  getHrAlerts: async (): Promise<ApiResponse<HrAlertsDto>> => {
    const res = await apiClient.get<ApiResponse<HrAlertsDto>>('/Report/hr-alerts');
    return res.data;
  },
};
