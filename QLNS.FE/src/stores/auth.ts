import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { authApi } from '@/api/auth.api';
import type { LoginDto, RegisterDto, GetCurrentUserDto } from '@/types';
import router from '@/router';

export type AppRole = 'Admin' | 'Accountant' | 'Employee';

export function normalizeAppRole(rawRole?: string | null): AppRole {
  const lower = (rawRole || '').trim().toLowerCase();
  if (lower === 'admin') return 'Admin';
  if (lower === 'accountant' || lower === 'kế toán' || lower === 'ke toan') return 'Accountant';
  return 'Employee';
}

export const useAuthStore = defineStore('auth', () => {
  const getInitialUser = (): GetCurrentUserDto | null => {
    try {
      const saved = localStorage.getItem('user_info');
      return saved ? JSON.parse(saved) : null;
    } catch {
      return null;
    }
  };

  const token = ref<string | null>(localStorage.getItem('access_token'));
  const user = ref<GetCurrentUserDto | null>(getInitialUser());
  const loading = ref(false);

  const isAuthenticated = computed(() => !!token.value);
  const normalizedRole = computed<AppRole>(() => normalizeAppRole(user.value?.role));
  const isAdmin = computed(() => normalizedRole.value === 'Admin');
  const isAccountant = computed(() => normalizedRole.value === 'Accountant');
  const isEmployee = computed(() => normalizedRole.value === 'Employee');
  const canManagePayroll = computed(() => isAdmin.value || isAccountant.value);

  const hasAnyRole = (roles?: AppRole[]) => {
    if (!roles || roles.length === 0) return true;
    return roles.includes(normalizedRole.value);
  };

  const needsFaceRegistration = computed(
    () => isAuthenticated.value && !!user.value && user.value.hasRegisteredFace === false
  );

  const setAuthData = (accessToken: string, refreshToken?: string) => {
    token.value = accessToken;
    localStorage.setItem('access_token', accessToken);
    if (refreshToken) {
      localStorage.setItem('refresh_token', refreshToken);
    }
  };

  const setUser = (userData: GetCurrentUserDto) => {
    const normalized: GetCurrentUserDto = {
      ...userData,
      role: normalizeAppRole(userData.role),
    };
    user.value = normalized;
    localStorage.setItem('user_info', JSON.stringify(normalized));
  };

  const markFaceRegistered = (faceImageUrl?: string | null) => {
    if (user.value) {
      setUser({
        ...user.value,
        hasRegisteredFace: true,
        faceImageUrl: faceImageUrl ?? user.value.faceImageUrl,
      });
    }
  };

  const login = async (dto: LoginDto) => {
    loading.value = true;
    try {
      const res = await authApi.login(dto);
      if (res.isSuccess && res.data) {
        setAuthData(res.data.accessToken, res.data.refreshToken);
        await fetchCurrentUser();
        return true;
      }
      return false;
    } finally {
      loading.value = false;
    }
  };

  const register = async (dto: RegisterDto) => {
    loading.value = true;
    try {
      const res = await authApi.register(dto);
      if (res.isSuccess && res.data) {
        setAuthData(res.data.accessToken, res.data.refreshToken);
        await fetchCurrentUser();
        return true;
      }
      return false;
    } finally {
      loading.value = false;
    }
  };

  const fetchCurrentUser = async () => {
    try {
      const res = await authApi.getCurrentUser();
      if (res.isSuccess && res.data) {
        setUser(res.data);
      }
    } catch (error) {
      console.warn('Failed to fetch current user profile:', error);
    }
  };

  const logout = () => {
    token.value = null;
    user.value = null;
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user_info');
    router.push('/login');
  };

  const initAuth = async () => {
    if (token.value) {
      await fetchCurrentUser();
    }
  };

  return {
    token,
    user,
    loading,
    isAuthenticated,
    normalizedRole,
    isAdmin,
    isAccountant,
    isEmployee,
    canManagePayroll,
    hasAnyRole,
    needsFaceRegistration,
    login,
    register,
    fetchCurrentUser,
    markFaceRegistered,
    logout,
    initAuth,
  };
});
