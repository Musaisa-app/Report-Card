using System;

namespace AbuAshraf.Application.DTOs
{
    /// <summary>
    /// DTO for JWT settings
    /// </summary>
    public class JwtSettings
    {
        public string Secret { get; set; }
        public int ExpiryInMinutes { get; set; }
        public int RefreshTokenExpiryInDays { get; set; } = 7;
    }

    /// <summary>
    /// DTO for JWT token response
    /// </summary>
    public class TokenResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = "Bearer";
    }

    /// <summary>
    /// DTO for refresh token request
    /// </summary>
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// DTO for email verification
    /// </summary>
    public class VerifyEmailDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }

    /// <summary>
    /// DTO for resend verification email
    /// </summary>
    public class ResendVerificationEmailDto
    {
        public string Email { get; set; }
    }

    /// <summary>
    /// DTO for change password
    /// </summary>
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// DTO for account lockout response
    /// </summary>
    public class AccountLockedDto
    {
        public bool IsLocked { get; set; }
        public DateTime? LockExpiryTime { get; set; }
        public string Message { get; set; }
    }
}
