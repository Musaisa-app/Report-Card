using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbuAshraf.Application.DTOs;
using AbuAshraf.Application.Services;
using AbuAshraf.Domain.Entities;

namespace AbuAshraf.Infrastructure.Services
{
    /// <summary>
    /// Wallet service implementation
    /// </summary>
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletTransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;

        public WalletService(
            IWalletRepository walletRepository,
            IWalletTransactionRepository transactionRepository,
            IUserRepository userRepository)
        {
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
        }

        public async Task<WalletDto> GetWalletAsync(Guid userId)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                return null;

            return MapToWalletDto(wallet);
        }

        public async Task<decimal> GetBalanceAsync(Guid userId)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            return wallet?.Balance ?? 0m;
        }

        public async Task<decimal> GetAvailableBalanceAsync(Guid userId)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                return 0m;

            return wallet.Balance - wallet.HoldAmount;
        }

        public async Task<bool> FundWalletAsync(Guid userId, decimal amount, string transactionType)
        {
            if (amount <= 0)
                return false;

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                return false;

            // Create transaction record
            var transaction = new WalletTransaction
            {
                TransactionId = Guid.NewGuid(),
                UserId = userId,
                WalletId = wallet.WalletId,
                ReferenceNumber = GenerateReferenceNumber(),
                TransactionType = transactionType,
                Amount = amount,
                PreviousBalance = wallet.Balance,
                NewBalance = wallet.Balance + amount,
                Status = "Success",
                Description = $"{transactionType} - Wallet funded",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            // Update wallet balance
            wallet.Balance += amount;
            wallet.AvailableBalance = wallet.Balance - wallet.HoldAmount;
            wallet.UpdatedDate = DateTime.UtcNow;

            // Save transaction and wallet
            await _transactionRepository.CreateAsync(transaction);
            await _walletRepository.UpdateAsync(wallet);

            return true;
        }

        public async Task<bool> DebitWalletAsync(Guid userId, decimal amount, string transactionType, string description)
        {
            if (amount <= 0)
                return false;

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                return false;

            // Check if sufficient balance
            if (wallet.AvailableBalance < amount)
                return false;

            // Check daily limit
            var dailySpentToday = await _transactionRepository.GetDailySpentAsync(userId, DateTime.UtcNow.Date);
            if (dailySpentToday + amount > wallet.DailyLimit)
                return false;

            // Check monthly limit
            var monthlySpent = await _transactionRepository.GetMonthlySpentAsync(userId, DateTime.UtcNow.Year, DateTime.UtcNow.Month);
            if (monthlySpent + amount > wallet.MonthlyLimit)
                return false;

            // Create transaction record
            var transaction = new WalletTransaction
            {
                TransactionId = Guid.NewGuid(),
                UserId = userId,
                WalletId = wallet.WalletId,
                ReferenceNumber = GenerateReferenceNumber(),
                TransactionType = transactionType,
                Amount = amount,
                PreviousBalance = wallet.Balance,
                NewBalance = wallet.Balance - amount,
                Status = "Success",
                Description = description,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            // Update wallet balance
            wallet.Balance -= amount;
            wallet.DailySpent += amount;
            wallet.MonthlySpent += amount;
            wallet.AvailableBalance = wallet.Balance - wallet.HoldAmount;
            wallet.UpdatedDate = DateTime.UtcNow;

            // Save transaction and wallet
            await _transactionRepository.CreateAsync(transaction);
            await _walletRepository.UpdateAsync(wallet);

            return true;
        }

        public async Task<bool> HoldAmountAsync(Guid userId, decimal amount)
        {
            if (amount <= 0)
                return false;

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                return false;

            if (wallet.AvailableBalance < amount)
                return false;

            wallet.HoldAmount += amount;
            wallet.AvailableBalance = wallet.Balance - wallet.HoldAmount;
            wallet.UpdatedDate = DateTime.UtcNow;

            await _walletRepository.UpdateAsync(wallet);
            return true;
        }

        public async Task<bool> ReleaseHoldAsync(Guid userId, decimal amount)
        {
            if (amount <= 0)
                return false;

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                return false;

            if (wallet.HoldAmount < amount)
                return false;

            wallet.HoldAmount -= amount;
            wallet.AvailableBalance = wallet.Balance - wallet.HoldAmount;
            wallet.UpdatedDate = DateTime.UtcNow;

            await _walletRepository.UpdateAsync(wallet);
            return true;
        }

        public async Task<TransactionHistoryDto> GetTransactionHistoryAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId, pageNumber, pageSize);
            var totalCount = await _transactionRepository.GetCountByUserIdAsync(userId);

            return new TransactionHistoryDto
            {
                Transactions = transactions.Select(MapToWalletTransactionDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<WalletDashboardDto> GetDashboardSummaryAsync(Guid userId)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                return null;

            var totalPurchases = await _transactionRepository.GetCountByTypeAsync(userId, new[] { "Airtime", "Data", "Cable", "Electricity" });
            var totalSpending = await _transactionRepository.GetSumByTypeAsync(userId, new[] { "Airtime", "Data", "Cable", "Electricity" });
            var referralEarnings = await _transactionRepository.GetSumByTypeAsync(userId, new[] { "Commission" });

            return new WalletDashboardDto
            {
                Balance = wallet.Balance,
                AvailableBalance = wallet.AvailableBalance,
                TotalPurchases = totalPurchases,
                TotalSpending = totalSpending,
                ReferralEarnings = referralEarnings
            };
        }

        public async Task<bool> IsWithinDailyLimitAsync(Guid userId, decimal amount)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                return false;

            var dailySpent = await _transactionRepository.GetDailySpentAsync(userId, DateTime.UtcNow.Date);
            return dailySpent + amount <= wallet.DailyLimit;
        }

        public async Task<bool> IsWithinMonthlyLimitAsync(Guid userId, decimal amount)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                return false;

            var monthlySpent = await _transactionRepository.GetMonthlySpentAsync(userId, DateTime.UtcNow.Year, DateTime.UtcNow.Month);
            return monthlySpent + amount <= wallet.MonthlyLimit;
        }

        public async Task<bool> HasSufficientBalanceAsync(Guid userId, decimal amount)
        {
            var balance = await GetAvailableBalanceAsync(userId);
            return balance >= amount;
        }

        public async Task<WalletTransactionDto> GetTransactionByReferenceAsync(string referenceNumber)
        {
            var transaction = await _transactionRepository.GetByReferenceNumberAsync(referenceNumber);
            return transaction != null ? MapToWalletTransactionDto(transaction) : null;
        }

        public async Task<bool> UpdateTransactionStatusAsync(string referenceNumber, string newStatus)
        {
            var transaction = await _transactionRepository.GetByReferenceNumberAsync(referenceNumber);
            if (transaction == null)
                return false;

            transaction.Status = newStatus;
            transaction.UpdatedDate = DateTime.UtcNow;
            await _transactionRepository.UpdateAsync(transaction);

            return true;
        }

        private string GenerateReferenceNumber()
        {
            return $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        private WalletDto MapToWalletDto(Wallet wallet)
        {
            return new WalletDto
            {
                WalletId = wallet.WalletId,
                UserId = wallet.UserId,
                Balance = wallet.Balance,
                AvailableBalance = wallet.AvailableBalance,
                HoldAmount = wallet.HoldAmount,
                Currency = wallet.Currency,
                UpdatedDate = wallet.UpdatedDate
            };
        }

        private WalletTransactionDto MapToWalletTransactionDto(WalletTransaction transaction)
        {
            return new WalletTransactionDto
            {
                TransactionId = transaction.TransactionId,
                ReferenceNumber = transaction.ReferenceNumber,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                Status = transaction.Status,
                Description = transaction.Description,
                CreatedDate = transaction.CreatedDate
            };
        }
    }
}
