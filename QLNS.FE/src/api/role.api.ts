import apiClient from './client';
import type {
  ApiResponse,
  ListItems,
  IdentityRole,
  UserInfoDto,
  AssignRoleDto,
  AssignClaimDto,
} from '@/types';

export const roleApi = {
  getAllRoles: async (): Promise<ApiResponse<ListItems<IdentityRole>>> => {
    const res = await apiClient.get<ApiResponse<ListItems<IdentityRole>>>('/Role');
    return res.data;
  },

  createRole: async (roleName: string): Promise<ApiResponse<IdentityRole>> => {
    const res = await apiClient.post<ApiResponse<IdentityRole>>('/Role', JSON.stringify(roleName), {
      headers: { 'Content-Type': 'application/json' },
    });
    return res.data;
  },

  deleteRole: async (roleName: string): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.delete<ApiResponse<boolean>>(`/Role/${encodeURIComponent(roleName)}`);
    return res.data;
  },

  updateRoleById: async (id: string, name: string): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.put<ApiResponse<boolean>>(`/Role?Id=${id}`, { name });
    return res.data;
  },

  getAllUsers: async (): Promise<ApiResponse<ListItems<UserInfoDto>>> => {
    const res = await apiClient.get<ApiResponse<ListItems<UserInfoDto>>>('/Role/users');
    return res.data;
  },

  assignRole: async (dto: AssignRoleDto): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.post<ApiResponse<boolean>>('/Role/assign-role', dto);
    return res.data;
  },

  updateUserRole: async (userId: string, assignRole: string): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.put<ApiResponse<boolean>>(`/Role/user-role?Id=${userId}`, JSON.stringify(assignRole), {
      headers: { 'Content-Type': 'application/json' },
    });
    return res.data;
  },

  assignClaim: async (dto: AssignClaimDto): Promise<ApiResponse<boolean>> => {
    const res = await apiClient.post<ApiResponse<boolean>>('/Role/assign-claim', dto);
    return res.data;
  },
};
