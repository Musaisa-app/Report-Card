using System;
using System.Threading.Tasks;
using AbuAshraf.Domain.Entities;

namespace AbuAshraf.Infrastructure.Services
{
    /// <summary>
    /// Repository interface for wallet operations
    /// </summary>
    public interface IWalletRepository
    {
        Task<Wallet> GetByIdAsync(Guid walletId);
        Task<Wallet> GetByUserIdAsync(Guid userId);
        Task CreateAsync(Wallet wallet);
        Task UpdateAsync(Wallet wallet);
        Task<bool> DeleteAsync(Guid walletId);
    }
}
