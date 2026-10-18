import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { authApi } from '@/api/auth.api';
import type { LoginDto, RegisterDto, GetCurrentUserDto } from '@/types';
import router from '@/router';

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
  const isAdmin = computed(() => user.value?.role?.toLowerCase() === 'admin');
  const isLeader = computed(() => user.value?.role?.toLowerCase() === 'leader');

  const setAuthData = (accessToken: string, refreshToken?: string) => {
    token.value = accessToken;
    localStorage.setItem('access_token', accessToken);
    if (refreshToken) {
      localStorage.setItem('refresh_token', refreshToken);
    }
  };

  const setUser = (userData: GetCurrentUserDto) => {
    user.value = userData;
    localStorage.setItem('user_info', JSON.stringify(userData));
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
    if (token.value && !user.value) {
      await fetchCurrentUser();
    }
  };

  return {
    token,
    user,
    loading,
    isAuthenticated,
    isAdmin,
    isLeader,
    login,
    register,
    fetchCurrentUser,
    logout,
    initAuth,
  };
});
