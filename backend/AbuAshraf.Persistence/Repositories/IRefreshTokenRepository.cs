using System;
using System.Threading.Tasks;
using AbuAshraf.Domain.Entities;

namespace AbuAshraf.Infrastructure.Services
{
    /// <summary>
    /// Repository interface for refresh token operations
    /// </summary>
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task CreateAsync(RefreshToken token);
        Task UpdateAsync(RefreshToken token);
        Task<RefreshToken> GetByIdAsync(Guid tokenId);
    }
}
