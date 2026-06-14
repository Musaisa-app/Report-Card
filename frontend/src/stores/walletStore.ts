import { create } from 'zustand';
import { walletService } from '@/services/walletService';
import { WalletDto, WalletDashboardDto } from '@/types/wallet';

interface WalletStore {
  wallet: WalletDto | null;
  dashboard: WalletDashboardDto | null;
  isLoading: boolean;
  error: string | null;
  refreshWallet: () => Promise<void>;
  refreshDashboard: () => Promise<void>;
  fundWallet: (amount: number, paymentGateway: string) => Promise<void>;
  verifyBalance: (amount: number) => Promise<boolean>;
  clearError: () => void;
}

export const useWalletStore = create<WalletStore>((set, get) => ({
  wallet: null,
  dashboard: null,
  isLoading: false,
  error: null,

  refreshWallet: async () => {
    set({ isLoading: true, error: null });
    try {
      const wallet = await walletService.getWallet();
      set({ wallet });
    } catch (error) {
      set({ error: error instanceof Error ? error.message : 'Failed to fetch wallet' });
    } finally {
      set({ isLoading: false });
    }
  },

  refreshDashboard: async () => {
    set({ isLoading: true, error: null });
    try {
      const dashboard = await walletService.getDashboard();
      set({ dashboard });
    } catch (error) {
      set({ error: error instanceof Error ? error.message : 'Failed to fetch dashboard' });
    } finally {
      set({ isLoading: false });
    }
  },

  fundWallet: async (amount: number, paymentGateway: string) => {
    set({ isLoading: true, error: null });
    try {
      const response = await walletService.fundWallet(amount, paymentGateway);
      if (response.success) {
        await get().refreshWallet();
      } else {
        set({ error: response.message });
      }
    } catch (error) {
      set({ error: error instanceof Error ? error.message : 'Failed to fund wallet' });
    } finally {
      set({ isLoading: false });
    }
  },

  verifyBalance: async (amount: number) => {
    try {
      const result = await walletService.verifyBalance(amount);
      return result.data.canProceed;
    } catch (error) {
      set({ error: error instanceof Error ? error.message : 'Balance verification failed' });
      return false;
    }
  },

  clearError: () => {
    set({ error: null });
  },
}));
