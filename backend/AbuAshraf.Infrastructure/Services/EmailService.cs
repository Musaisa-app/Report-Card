using System;
using System.Threading.Tasks;
using AbuAshraf.Application.Services;

namespace AbuAshraf.Infrastructure.Services
{
    /// <summary>
    /// Email service implementation (stub for future integration)
    /// </summary>
    public class EmailService : IEmailService
    {
        // TODO: Implement with SendGrid, AWS SES, or similar
        // For now, logs to console in development

        public async Task SendWelcomeEmailAsync(string email, string fullName)
        {
            Console.WriteLine($"[EMAIL] Welcome email sent to {email} for {fullName}");
            await Task.CompletedTask;
        }

        public async Task SendVerificationEmailAsync(string email, string verificationLink)
        {
            Console.WriteLine($"[EMAIL] Verification email sent to {email}");
            Console.WriteLine($"[EMAIL] Verification link: {verificationLink}");
            await Task.CompletedTask;
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            Console.WriteLine($"[EMAIL] Password reset email sent to {email}");
            Console.WriteLine($"[EMAIL] Reset link: {resetLink}");
            await Task.CompletedTask;
        }

        public async Task SendPasswordChangedEmailAsync(string email)
        {
            Console.WriteLine($"[EMAIL] Password changed notification sent to {email}");
            await Task.CompletedTask;
        }

        public async Task SendAccountLockedEmailAsync(string email)
        {
            Console.WriteLine($"[EMAIL] Account locked notification sent to {email}");
            await Task.CompletedTask;
        }

        public async Task SendLoginNotificationEmailAsync(string email, string ipAddress)
        {
            Console.WriteLine($"[EMAIL] Login notification sent to {email} from IP: {ipAddress}");
            await Task.CompletedTask;
        }
    }
}
