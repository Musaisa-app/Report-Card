using System;
using System.Threading.Tasks;
using AbuAshraf.Application.DTOs;

namespace AbuAshraf.Application.Services
{
    /// <summary>
    /// Service interface for authentication operations
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Register new user
        /// </summary>
        Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);

        /// <summary>
        /// Authenticate user with email or phone
        /// </summary>
        Task<AuthResponseDto> LoginAsync(LoginUserDto dto);

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress);

        /// <summary>
        /// Verify email address
        /// </summary>
        Task<bool> VerifyEmailAsync(string email, string token);

        /// <summary>
        /// Resend email verification
        /// </summary>
        Task<bool> ResendVerificationEmailAsync(string email);

        /// <summary>
        /// Request password reset
        /// </summary>
        Task<bool> RequestPasswordResetAsync(string email);

        /// <summary>
        /// Reset password
        /// </summary>
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

        /// <summary>
        /// Change password for authenticated user
        /// </summary>
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);

        /// <summary>
        /// Revoke refresh token (logout)
        /// </summary>
        Task<bool> LogoutAsync(string refreshToken);

        /// <summary>
        /// Check if account is locked
        /// </summary>
        Task<bool> IsAccountLockedAsync(Guid userId);

        /// <summary>
        /// Unlock account
        /// </summary>
        Task<bool> UnlockAccountAsync(Guid userId);

        /// <summary>
        /// Verify referral code
        /// </summary>
        Task<Guid?> GetUserByReferralCodeAsync(string referralCode);
    }
}
