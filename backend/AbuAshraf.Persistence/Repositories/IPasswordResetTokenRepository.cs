using System;
using System.Threading.Tasks;
using AbuAshraf.Domain.Entities;

namespace AbuAshraf.Infrastructure.Services
{
    /// <summary>
    /// Repository interface for password reset token operations
    /// </summary>
    public interface IPasswordResetTokenRepository
    {
        Task<PasswordResetToken> GetByTokenAsync(string token);
        Task CreateAsync(PasswordResetToken token);
        Task UpdateAsync(PasswordResetToken token);
    }
}
