using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AbuAshraf.Application.Commands;
using AbuAshraf.Application.DTOs;
using AbuAshraf.Application.Services;

namespace AbuAshraf.API.Controllers
{
    /// <summary>
    /// Authentication endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthenticationService _authService;

        public AuthController(IMediator mediator, IAuthenticationService authService)
        {
            _mediator = mediator;
            _authService = authService;
        }

        /// <summary>
        /// Register new user
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            try
            {
                var command = new RegisterUserCommand(
                    dto,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers["User-Agent"].ToString()
                );

                var result = await _mediator.Send(command);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Login user
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            try
            {
                var command = new LoginUserCommand(
                    dto,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    HttpContext.Request.Headers["User-Agent"].ToString()
                );

                var result = await _mediator.Send(command);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Refresh access token
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            try
            {
                var command = new RefreshTokenCommand(
                    dto.RefreshToken,
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                );

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Verify email address
        /// </summary>
        [HttpPost("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
        {
            try
            {
                var result = await _authService.VerifyEmailAsync(dto.Email, dto.Token);
                return result
                    ? Ok(new { Success = true, Message = "Email verified successfully" })
                    : BadRequest(new { Success = false, Message = "Invalid or expired token" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Resend email verification
        /// </summary>
        [HttpPost("resend-verification-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendVerificationEmailDto dto)
        {
            try
            {
                var result = await _authService.ResendVerificationEmailAsync(dto.Email);
                return result
                    ? Ok(new { Success = true, Message = "Verification email sent" })
                    : BadRequest(new { Success = false, Message = "User not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Request password reset
        /// </summary>
        [HttpPost("request-password-reset")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto dto)
        {
            try
            {
                await _authService.RequestPasswordResetAsync(dto.Email);
                return Ok(new { Success = true, Message = "Password reset link sent to email" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Reset password
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto dto)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(dto.Email, dto.Token, dto.NewPassword);
                return result
                    ? Ok(new { Success = true, Message = "Password reset successfully" })
                    : BadRequest(new { Success = false, Message = "Invalid or expired token" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Change password (authenticated)
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
                var result = await _authService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
                return result
                    ? Ok(new { Success = true, Message = "Password changed successfully" })
                    : BadRequest(new { Success = false, Message = "Current password is incorrect" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Logout (revoke refresh token)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
        {
            try
            {
                await _authService.LogoutAsync(dto.RefreshToken);
                return Ok(new { Success = true, Message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult Health()
        {
            return Ok(new { Status = "OK", Timestamp = DateTime.UtcNow });
        }
    }
}
