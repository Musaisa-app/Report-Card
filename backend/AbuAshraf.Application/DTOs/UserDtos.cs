using System;

namespace AbuAshraf.Application.DTOs
{
    /// <summary>
    /// DTO for user registration
    /// </summary>
    public class RegisterUserDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string ReferralCode { get; set; } // Optional referrer's code
    }

    /// <summary>
    /// DTO for user login
    /// </summary>
    public class LoginUserDto
    {
        public string EmailOrPhone { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// DTO for authentication response
    /// </summary>
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserDto User { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// DTO for user information
    /// </summary>
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; }
        public string ReferralCode { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// DTO for password reset request
    /// </summary>
    public class PasswordResetRequestDto
    {
        public string Email { get; set; }
    }

    /// <summary>
    /// DTO for password reset
    /// </summary>
    public class PasswordResetDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
