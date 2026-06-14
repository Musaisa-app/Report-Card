export interface WalletDto {
  walletId: string;
  userId: string;
  balance: number;
  availableBalance: number;
  holdAmount: number;
  currency: string;
  updatedDate: string;
}

export interface WalletTransactionDto {
  transactionId: string;
  referenceNumber: string;
  transactionType: string;
  amount: number;
  status: string;
  description: string;
  createdDate: string;
}

export interface TransactionHistoryDto {
  transactions: WalletTransactionDto[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface WalletDashboardDto {
  balance: number;
  availableBalance: number;
  totalPurchases: number;
  totalSpending: number;
  referralEarnings: number;
}

export interface FundWalletDto {
  amount: number;
  paymentGateway: string;
}

export interface VerifyBalanceDto {
  amount: number;
}
