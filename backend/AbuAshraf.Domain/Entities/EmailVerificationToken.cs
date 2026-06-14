using System;

namespace AbuAshraf.Domain.Entities
{
    /// <summary>
    /// Email verification token for email confirmation
    /// </summary>
    public class EmailVerificationToken
    {
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UsedDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
}
