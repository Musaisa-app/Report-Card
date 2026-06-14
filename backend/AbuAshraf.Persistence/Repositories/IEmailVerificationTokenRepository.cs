using System;
using System.Threading.Tasks;
using AbuAshraf.Domain.Entities;

namespace AbuAshraf.Infrastructure.Services
{
    /// <summary>
    /// Repository interface for email verification token operations
    /// </summary>
    public interface IEmailVerificationTokenRepository
    {
        Task<EmailVerificationToken> GetByTokenAsync(string token);
        Task CreateAsync(EmailVerificationToken token);
        Task UpdateAsync(EmailVerificationToken token);
    }
}
