import apiClient from './client';
import type {
  ApiResponse,
  LoginDto,
  RegisterDto,
  TokenDto,
  GetCurrentUserDto,
} from '@/types';

export const authApi = {
  login: async (dto: LoginDto): Promise<ApiResponse<TokenDto>> => {
    const res = await apiClient.post<ApiResponse<TokenDto>>('/Auth/login', dto);
    return res.data;
  },

  register: async (dto: RegisterDto): Promise<ApiResponse<TokenDto>> => {
    const res = await apiClient.post<ApiResponse<TokenDto>>('/Auth/register', dto);
    return res.data;
  },

  getCurrentUser: async (): Promise<ApiResponse<GetCurrentUserDto>> => {
    const res = await apiClient.get<ApiResponse<GetCurrentUserDto>>('/Auth/getCurrentUser');
    return res.data;
  },

  refreshToken: async (accessToken: string, refreshToken: string) => {
    const res = await apiClient.post('/Auth/refresh', { accessToken, refreshToken });
    return res.data;
  },
};
