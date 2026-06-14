using System;
using System.Threading.Tasks;
using AbuAshraf.Domain.Entities;

namespace AbuAshraf.Infrastructure.Services
{
    /// <summary>
    /// Repository interface for user operations
    /// </summary>
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid userId);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByPhoneAsync(string phone);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByReferralCodeAsync(string referralCode);
        Task<bool> ReferralCodeExistsAsync(string referralCode);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task CreateWalletAsync(Wallet wallet);
        Task CreateReferralAsync(Referral referral);
    }
}
