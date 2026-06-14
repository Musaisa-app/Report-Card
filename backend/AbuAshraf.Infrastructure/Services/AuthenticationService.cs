using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AbuAshraf.Application.DTOs;
using AbuAshraf.Application.Services;
using AbuAshraf.Domain.Entities;

namespace AbuAshraf.Infrastructure.Services
{
    /// <summary>
    /// Authentication service implementation
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IEmailVerificationTokenRepository _emailVerificationRepository;
        private readonly IPasswordResetTokenRepository _passwordResetRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private const int MaxFailedLoginAttempts = 5;
        private const int AccountLockoutDurationMinutes = 30;

        public AuthenticationService(
            ITokenService tokenService,
            IEmailService emailService,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IEmailVerificationTokenRepository emailVerificationRepository,
            IPasswordResetTokenRepository passwordResetRepository)
        {
            _tokenService = tokenService;
            _emailService = emailService;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _emailVerificationRepository = emailVerificationRepository;
            _passwordResetRepository = passwordResetRepository;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto)
        {
            // Check if user already exists
            var existingUserByEmail = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUserByEmail != null)
                return new AuthResponseDto { Success = false, Message = "Email already registered" };

            var existingUserByPhone = await _userRepository.GetByPhoneAsync(dto.PhoneNumber);
            if (existingUserByPhone != null)
                return new AuthResponseDto { Success = false, Message = "Phone number already registered" };

            var existingUserByUsername = await _userRepository.GetByUsernameAsync(dto.Username);
            if (existingUserByUsername != null)
                return new AuthResponseDto { Success = false, Message = "Username already taken" };

            // Generate unique referral code
            string referralCode = GenerateReferralCode();
            while (await _userRepository.ReferralCodeExistsAsync(referralCode))
            {
                referralCode = GenerateReferralCode();
            }

            // Create new user
            var user = new User
            {
                UserId = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Username = dto.Username,
                RoleId = 4, // Customer role
                ReferralCode = referralCode,
                IsEmailVerified = false,
                Status = "Active",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            // Hash password
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            // Create wallet for user
            var wallet = new Wallet
            {
                WalletId = Guid.NewGuid(),
                UserId = user.UserId,
                Balance = 0,
                AvailableBalance = 0,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            // Handle referral
            if (!string.IsNullOrEmpty(dto.ReferralCode))
            {
                var referrer = await _userRepository.GetByReferralCodeAsync(dto.ReferralCode);
                if (referrer != null)
                {
                    // Create referral record
                    var referral = new Referral
                    {
                        ReferralId = Guid.NewGuid(),
                        ReferrerId = referrer.UserId,
                        RefereeId = user.UserId,
                        CommissionRate = 1.0m,
                        CreatedDate = DateTime.UtcNow
                    };
                    await _userRepository.CreateReferralAsync(referral);
                }
            }

            // Save user and wallet
            await _userRepository.CreateAsync(user);
            await _userRepository.CreateWalletAsync(wallet);

            // Generate email verification token
            var verificationToken = GenerateToken();
            var emailVerificationToken = new EmailVerificationToken
            {
                TokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = verificationToken,
                ExpiryDate = DateTime.UtcNow.AddHours(24),
                IsUsed = false,
                CreatedDate = DateTime.UtcNow
            };
            await _emailVerificationRepository.CreateAsync(emailVerificationToken);

            // Send verification email
            var verificationLink = $"https://app.abu-ashraf.com/verify-email?token={verificationToken}&email={dto.Email}";
            await _emailService.SendVerificationEmailAsync(dto.Email, verificationLink);
            await _emailService.SendWelcomeEmailAsync(dto.Email, dto.FullName);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful. Please verify your email.",
                User = MapToUserDto(user)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginUserDto dto)
        {
            // Find user by email or phone
            User user = null;
            if (dto.EmailOrPhone.Contains("@"))
                user = await _userRepository.GetByEmailAsync(dto.EmailOrPhone);
            else
                user = await _userRepository.GetByPhoneAsync(dto.EmailOrPhone);

            if (user == null)
                return new AuthResponseDto { Success = false, Message = "Invalid credentials" };

            // Check if account is locked
            if (await IsAccountLockedAsync(user.UserId))
                return new AuthResponseDto { Success = false, Message = "Account is locked. Try again later." };

            // Verify password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= MaxFailedLoginAttempts)
                {
                    user.AccountLocked = true;
                    await _emailService.SendAccountLockedEmailAsync(user.Email);
                }
                await _userRepository.UpdateAsync(user);
                return new AuthResponseDto { Success = false, Message = "Invalid credentials" };
            }

            // Reset failed attempts and update last login
            user.FailedLoginAttempts = 0;
            user.AccountLocked = false;
            user.LastLogin = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.UserId, "127.0.0.1", "Web");
            await _refreshTokenRepository.CreateAsync(refreshToken);

            // Send login notification
            await _emailService.SendLoginNotificationEmailAsync(user.Email, "127.0.0.1");

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                User = MapToUserDto(user),
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress)
        {
            var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedRefreshToken == null || storedRefreshToken.IsRevoked || storedRefreshToken.ExpiryDate < DateTime.UtcNow)
                throw new Exception("Invalid or expired refresh token");

            var user = await _userRepository.GetByIdAsync(storedRefreshToken.UserId);
            if (user == null)
                throw new Exception("User not found");

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user.UserId, ipAddress, "Web");

            // Revoke old token
            storedRefreshToken.IsRevoked = true;
            storedRefreshToken.RevokedDate = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(storedRefreshToken);

            // Save new token
            await _refreshTokenRepository.CreateAsync((RefreshToken)newRefreshToken);

            return new TokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = ((RefreshToken)newRefreshToken).Token,
                ExpiresIn = 3600
            };
        }

        public async Task<bool> VerifyEmailAsync(string email, string token)
        {
            var verificationToken = await _emailVerificationRepository.GetByTokenAsync(token);
            if (verificationToken == null || verificationToken.IsUsed || verificationToken.ExpiryDate < DateTime.UtcNow)
                return false;

            var user = await _userRepository.GetByIdAsync(verificationToken.UserId);
            if (user == null || user.Email != email)
                return false;

            user.IsEmailVerified = true;
            await _userRepository.UpdateAsync(user);

            verificationToken.IsUsed = true;
            verificationToken.UsedDate = DateTime.UtcNow;
            await _emailVerificationRepository.UpdateAsync(verificationToken);

            return true;
        }

        public async Task<bool> ResendVerificationEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return false;

            var verificationToken = GenerateToken();
            var emailVerificationToken = new EmailVerificationToken
            {
                TokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = verificationToken,
                ExpiryDate = DateTime.UtcNow.AddHours(24),
                IsUsed = false,
                CreatedDate = DateTime.UtcNow
            };
            await _emailVerificationRepository.CreateAsync(emailVerificationToken);

            var verificationLink = $"https://app.abu-ashraf.com/verify-email?token={verificationToken}&email={email}";
            await _emailService.SendVerificationEmailAsync(email, verificationLink);

            return true;
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return true; // Don't reveal if email exists

            var resetToken = GenerateToken();
            var passwordResetToken = new PasswordResetToken
            {
                TokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = resetToken,
                ExpiryDate = DateTime.UtcNow.AddHours(1),
                IsUsed = false,
                CreatedDate = DateTime.UtcNow
            };
            await _passwordResetRepository.CreateAsync(passwordResetToken);

            var resetLink = $"https://app.abu-ashraf.com/reset-password?token={resetToken}&email={email}";
            await _emailService.SendPasswordResetEmailAsync(email, resetLink);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var resetToken = await _passwordResetRepository.GetByTokenAsync(token);
            if (resetToken == null || resetToken.IsUsed || resetToken.ExpiryDate < DateTime.UtcNow)
                return false;

            var user = await _userRepository.GetByIdAsync(resetToken.UserId);
            if (user == null || user.Email != email)
                return false;

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            user.FailedLoginAttempts = 0;
            user.AccountLocked = false;
            await _userRepository.UpdateAsync(user);

            resetToken.IsUsed = true;
            resetToken.UsedDate = DateTime.UtcNow;
            await _passwordResetRepository.UpdateAsync(resetToken);

            await _emailService.SendPasswordChangedEmailAsync(email);

            return true;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
                return false;

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await _userRepository.UpdateAsync(user);

            await _emailService.SendPasswordChangedEmailAsync(user.Email);

            return true;
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (token == null)
                return false;

            token.IsRevoked = true;
            token.RevokedDate = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(token);

            return true;
        }

        public async Task<bool> IsAccountLockedAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null && user.AccountLocked;
        }

        public async Task<bool> UnlockAccountAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.AccountLocked = false;
            user.FailedLoginAttempts = 0;
            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<Guid?> GetUserByReferralCodeAsync(string referralCode)
        {
            var user = await _userRepository.GetByReferralCodeAsync(referralCode);
            return user?.UserId;
        }

        private string GenerateReferralCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Range(0, 8)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }

        private string GenerateToken()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ReferralCode = user.ReferralCode,
                IsEmailVerified = user.IsEmailVerified,
                IsPhoneVerified = user.IsPhoneVerified,
                Status = user.Status,
                CreatedDate = user.CreatedDate
            };
        }
    }
}
