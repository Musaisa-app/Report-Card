import apiClient from '@/config/api';
import { RegisterUserDto, LoginUserDto, TokenResponseDto, AuthResponseDto } from '@/types/auth';

export const authService = {
  register: async (data: RegisterUserDto): Promise<AuthResponseDto> => {
    const response = await apiClient.post('/api/auth/register', data);
    return response.data;
  },

  login: async (data: LoginUserDto): Promise<AuthResponseDto> => {
    const response = await apiClient.post('/api/auth/login', data);
    if (response.data.success) {
      localStorage.setItem('accessToken', response.data.accessToken);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      localStorage.setItem('user', JSON.stringify(response.data.user));
    }
    return response.data;
  },

  logout: async (): Promise<void> => {
    const refreshToken = localStorage.getItem('refreshToken');
    if (refreshToken) {
      try {
        await apiClient.post('/api/auth/logout', { refreshToken });
      } catch (error) {
        console.error('Logout error:', error);
      }
    }
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  },

  refreshToken: async (refreshToken: string): Promise<TokenResponseDto> => {
    const response = await apiClient.post('/api/auth/refresh-token', { refreshToken });
    if (response.data.accessToken) {
      localStorage.setItem('accessToken', response.data.accessToken);
      localStorage.setItem('refreshToken', response.data.refreshToken);
    }
    return response.data;
  },

  verifyEmail: async (email: string, token: string): Promise<{ success: boolean; message: string }> => {
    const response = await apiClient.post('/api/auth/verify-email', { email, token });
    return response.data;
  },

  resendVerificationEmail: async (email: string): Promise<{ success: boolean; message: string }> => {
    const response = await apiClient.post('/api/auth/resend-verification-email', { email });
    return response.data;
  },

  requestPasswordReset: async (email: string): Promise<{ success: boolean; message: string }> => {
    const response = await apiClient.post('/api/auth/request-password-reset', { email });
    return response.data;
  },

  resetPassword: async (email: string, token: string, newPassword: string): Promise<{ success: boolean; message: string }> => {
    const response = await apiClient.post('/api/auth/reset-password', {
      email,
      token,
      newPassword,
      confirmPassword: newPassword,
    });
    return response.data;
  },

  changePassword: async (currentPassword: string, newPassword: string): Promise<{ success: boolean; message: string }> => {
    const response = await apiClient.post('/api/auth/change-password', {
      currentPassword,
      newPassword,
      confirmPassword: newPassword,
    });
    return response.data;
  },

  getCurrentUser: () => {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  },

  isAuthenticated: (): boolean => {
    return !!localStorage.getItem('accessToken');
  },
};
