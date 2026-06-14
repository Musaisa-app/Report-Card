using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbuAshraf.Application.DTOs;
using AbuAshraf.Domain.Entities;

namespace AbuAshraf.Application.Services
{
    /// <summary>
    /// Service interface for wallet operations
    /// </summary>
    public interface IWalletService
    {
        /// <summary>
        /// Get wallet by user ID
        /// </summary>
        Task<WalletDto> GetWalletAsync(Guid userId);

        /// <summary>
        /// Get wallet balance
        /// </summary>
        Task<decimal> GetBalanceAsync(Guid userId);

        /// <summary>
        /// Get available balance (balance - hold amount)
        /// </summary>
        Task<decimal> GetAvailableBalanceAsync(Guid userId);

        /// <summary>
        /// Fund wallet (add credit)
        /// </summary>
        Task<bool> FundWalletAsync(Guid userId, decimal amount, string transactionType);

        /// <summary>
        /// Debit wallet (subtract amount)
        /// </summary>
        Task<bool> DebitWalletAsync(Guid userId, decimal amount, string transactionType, string description);

        /// <summary>
        /// Hold amount for pending transaction
        /// </summary>
        Task<bool> HoldAmountAsync(Guid userId, decimal amount);

        /// <summary>
        /// Release held amount
        /// </summary>
        Task<bool> ReleaseHoldAsync(Guid userId, decimal amount);

        /// <summary>
        /// Get transaction history
        /// </summary>
        Task<TransactionHistoryDto> GetTransactionHistoryAsync(Guid userId, int pageNumber = 1, int pageSize = 10);

        /// <summary>
        /// Get wallet dashboard summary
        /// </summary>
        Task<WalletDashboardDto> GetDashboardSummaryAsync(Guid userId);

        /// <summary>
        /// Check if transaction is within daily limit
        /// </summary>
        Task<bool> IsWithinDailyLimitAsync(Guid userId, decimal amount);

        /// <summary>
        /// Check if transaction is within monthly limit
        /// </summary>
        Task<bool> IsWithinMonthlyLimitAsync(Guid userId, decimal amount);

        /// <summary>
        /// Verify sufficient balance
        /// </summary>
        Task<bool> HasSufficientBalanceAsync(Guid userId, decimal amount);

        /// <summary>
        /// Get transaction by reference number
        /// </summary>
        Task<WalletTransactionDto> GetTransactionByReferenceAsync(string referenceNumber);

        /// <summary>
        /// Update transaction status
        /// </summary>
        Task<bool> UpdateTransactionStatusAsync(string referenceNumber, string newStatus);
    }
}
