import apiClient from './client';
import type {
  ApiResponse,
  PagedResult,
  ListItems,
  AttendanceDto,
  AddAttendanceDto,
  FaceRegisterDto,
  FaceRegisterResultDto,
  FaceCheckInDto,
  FaceCheckInResultDto,
  QrTokenDto,
  QrCheckInResultDto,
} from '@/types';

export const attendanceApi = {
  getAll: async (): Promise<ApiResponse<ListItems<AttendanceDto>>> => {
    const res = await apiClient.get<ApiResponse<ListItems<AttendanceDto>>>('/Attendance');
    return res.data;
  },

  getFilter: async (pageIndex = 1, pageSize = 10, search?: string): Promise<ApiResponse<PagedResult<AttendanceDto>>> => {
    const params: Record<string, unknown> = { pageIndex, pageSize };
    if (search) params.search = search;
    const res = await apiClient.get<ApiResponse<PagedResult<AttendanceDto>>>('/Attendance/filter', { params });
    return res.data;
  },

  getById: async (id: string): Promise<ApiResponse<AttendanceDto>> => {
    const res = await apiClient.get<ApiResponse<AttendanceDto>>(`/Attendance/${id}`);
    return res.data;
  },

  add: async (dto: AddAttendanceDto): Promise<ApiResponse<AttendanceDto>> => {
    const res = await apiClient.post<ApiResponse<AttendanceDto>>('/Attendance', dto);
    return res.data;
  },

  update: async (id: string, dto: AddAttendanceDto): Promise<ApiResponse<AttendanceDto>> => {
    const res = await apiClient.put<ApiResponse<AttendanceDto>>(`/Attendance/${id}`, dto);
    return res.data;
  },

  delete: async (id: string): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.delete<ApiResponse<boolean>>(`/Attendance/${id}`);
    return res.data;
  },

  // B1: Đăng ký khuôn mặt lần đầu
  registerFace: async (dto: FaceRegisterDto): Promise<ApiResponse<FaceRegisterResultDto>> => {
    const res = await apiClient.post<ApiResponse<FaceRegisterResultDto>>('/Attendance/register-face', dto);
    return res.data;
  },

  // B3 & B4: Chụp khuôn mặt gửi về BE đối sánh và chấm công
  faceCheckIn: async (dto: FaceCheckInDto): Promise<ApiResponse<FaceCheckInResultDto>> => {
    const res = await apiClient.post<ApiResponse<FaceCheckInResultDto>>('/Attendance/face-checkin', dto);
    return res.data;
  },

  // B2 & B4: Báo cáo mã lỗi kết nối Camera về BE
  reportCameraError: async (errorCode: string, errorDetail?: string): Promise<ApiResponse<unknown>> => {
    const res = await apiClient.post<ApiResponse<unknown>>('/Attendance/camera-error', {
      errorCode,
      errorDetail,
    });
    return res.data;
  },

  getQrToken: async (): Promise<ApiResponse<QrTokenDto>> => {
    const res = await apiClient.get<ApiResponse<QrTokenDto>>('/Attendance/qr-token');
    return res.data;
  },

  qrCheckIn: async (qrToken: string): Promise<ApiResponse<QrCheckInResultDto>> => {
    const res = await apiClient.post<ApiResponse<QrCheckInResultDto>>('/Attendance/qr-checkin', { qrToken });
    return res.data;
  },

  getMyAttendance: async (month?: number, year?: number): Promise<ApiResponse<AttendanceDto[]>> => {
    const params: Record<string, unknown> = {};
    if (month) params.month = month;
    if (year) params.year = year;
    const res = await apiClient.get<ApiResponse<AttendanceDto[]>>('/Attendance/my-attendance', { params });
    return res.data;
  },
};
