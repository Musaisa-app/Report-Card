using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbuAshraf.Application.DTOs;
using AbuAshraf.Domain.Entities;

namespace AbuAshraf.Application.Services
{
    /// <summary>
    /// Service interface for token generation and management
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generate JWT access token
        /// </summary>
        string GenerateAccessToken(User user);

        /// <summary>
        /// Generate refresh token
        /// </summary>
        Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, string ipAddress, string userAgent);

        /// <summary>
        /// Validate JWT token
        /// </summary>
        bool ValidateToken(string token);

        /// <summary>
        /// Get claims from token
        /// </summary>
        Dictionary<string, object> GetTokenClaims(string token);

        /// <summary>
        /// Revoke refresh token
        /// </summary>
        Task RevokeRefreshTokenAsync(string token);
    }
}
