using System.Threading.Tasks;

namespace AbuAshraf.Application.Services
{
    /// <summary>
    /// Service interface for email notifications
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Send welcome email
        /// </summary>
        Task SendWelcomeEmailAsync(string email, string fullName);

        /// <summary>
        /// Send email verification
        /// </summary>
        Task SendVerificationEmailAsync(string email, string verificationLink);

        /// <summary>
        /// Send password reset email
        /// </summary>
        Task SendPasswordResetEmailAsync(string email, string resetLink);

        /// <summary>
        /// Send password changed notification
        /// </summary>
        Task SendPasswordChangedEmailAsync(string email);

        /// <summary>
        /// Send account locked notification
        /// </summary>
        Task SendAccountLockedEmailAsync(string email);

        /// <summary>
        /// Send login notification
        /// </summary>
        Task SendLoginNotificationEmailAsync(string email, string ipAddress);
    }
}
