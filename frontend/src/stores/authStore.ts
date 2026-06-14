import { create } from 'zustand';
import { authService } from '@/services/authService';
import { UserDto } from '@/types/auth';

interface AuthStore {
  user: UserDto | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  login: (emailOrPhone: string, password: string) => Promise<void>;
  register: (fullName: string, email: string, phoneNumber: string, username: string, password: string, referralCode?: string) => Promise<void>;
  logout: () => Promise<void>;
  setUser: (user: UserDto | null) => void;
  clearError: () => void;
}

export const useAuthStore = create<AuthStore>((set) => ({
  user: authService.getCurrentUser(),
  isAuthenticated: authService.isAuthenticated(),
  isLoading: false,
  error: null,

  login: async (emailOrPhone: string, password: string) => {
    set({ isLoading: true, error: null });
    try {
      const response = await authService.login({ emailOrPhone, password });
      if (response.success) {
        set({ user: response.user, isAuthenticated: true });
      } else {
        set({ error: response.message });
      }
    } catch (error) {
      set({ error: error instanceof Error ? error.message : 'Login failed' });
    } finally {
      set({ isLoading: false });
    }
  },

  register: async (fullName: string, email: string, phoneNumber: string, username: string, password: string, referralCode?: string) => {
    set({ isLoading: true, error: null });
    try {
      const response = await authService.register({
        fullName,
        email,
        phoneNumber,
        username,
        password,
        confirmPassword: password,
        referralCode,
      });
      if (response.success) {
        set({ user: response.user, isAuthenticated: false });
      } else {
        set({ error: response.message });
      }
    } catch (error) {
      set({ error: error instanceof Error ? error.message : 'Registration failed' });
    } finally {
      set({ isLoading: false });
    }
  },

  logout: async () => {
    set({ isLoading: true });
    try {
      await authService.logout();
      set({ user: null, isAuthenticated: false });
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      set({ isLoading: false });
    }
  },

  setUser: (user: UserDto | null) => {
    set({ user, isAuthenticated: !!user });
  },

  clearError: () => {
    set({ error: null });
  },
}));
