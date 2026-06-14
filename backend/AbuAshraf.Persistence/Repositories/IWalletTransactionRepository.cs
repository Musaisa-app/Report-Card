using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbuAshraf.Domain.Entities;

namespace AbuAshraf.Infrastructure.Services
{
    /// <summary>
    /// Repository interface for wallet transaction operations
    /// </summary>
    public interface IWalletTransactionRepository
    {
        Task<WalletTransaction> GetByIdAsync(Guid transactionId);
        Task<WalletTransaction> GetByReferenceNumberAsync(string referenceNumber);
        Task<List<WalletTransaction>> GetByUserIdAsync(Guid userId, int pageNumber, int pageSize);
        Task<int> GetCountByUserIdAsync(Guid userId);
        Task<decimal> GetDailySpentAsync(Guid userId, DateTime date);
        Task<decimal> GetMonthlySpentAsync(Guid userId, int year, int month);
        Task<int> GetCountByTypeAsync(Guid userId, string[] transactionTypes);
        Task<decimal> GetSumByTypeAsync(Guid userId, string[] transactionTypes);
        Task CreateAsync(WalletTransaction transaction);
        Task UpdateAsync(WalletTransaction transaction);
        Task<bool> DeleteAsync(Guid transactionId);
    }
}
