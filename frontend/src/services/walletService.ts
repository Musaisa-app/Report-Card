import apiClient from '@/config/api';
import { WalletDto, WalletTransactionDto, TransactionHistoryDto, WalletDashboardDto } from '@/types/wallet';

export const walletService = {
  getWallet: async (): Promise<WalletDto> => {
    const response = await apiClient.get('/api/wallet/balance');
    return response.data.data;
  },

  getAvailableBalance: async (): Promise<number> => {
    const response = await apiClient.get('/api/wallet/available-balance');
    return response.data.data.availableBalance;
  },

  getDashboard: async (): Promise<WalletDashboardDto> => {
    const response = await apiClient.get('/api/wallet/dashboard');
    return response.data.data;
  },

  getTransactionHistory: async (pageNumber: number = 1, pageSize: number = 10): Promise<TransactionHistoryDto> => {
    const response = await apiClient.get('/api/wallet/transactions', {
      params: { pageNumber, pageSize },
    });
    return response.data.data;
  },

  getTransaction: async (referenceNumber: string): Promise<WalletTransactionDto> => {
    const response = await apiClient.get(`/api/wallet/transactions/${referenceNumber}`);
    return response.data.data;
  },

  fundWallet: async (amount: number, paymentGateway: string): Promise<{ success: boolean; message: string }> => {
    const response = await apiClient.post('/api/wallet/fund', { amount, paymentGateway });
    return response.data;
  },

  verifyBalance: async (amount: number): Promise<{
    success: boolean;
    data: {
      hasSufficientBalance: boolean;
      withinDailyLimit: boolean;
      withinMonthlyLimit: boolean;
      canProceed: boolean;
    };
  }> => {
    const response = await apiClient.post('/api/wallet/verify-balance', { amount });
    return response.data;
  },
};
