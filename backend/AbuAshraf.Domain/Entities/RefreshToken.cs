using System;

namespace AbuAshraf.Domain.Entities
{
    /// <summary>
    /// Refresh token entity for token rotation and security
    /// </summary>
    public class RefreshToken
    {
        public Guid RefreshTokenId { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
}
