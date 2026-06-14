using System;

namespace AbuAshraf.Domain.Entities
{
    /// <summary>
    /// Referral entity for tracking referral bonuses
    /// </summary>
    public class Referral
    {
        public Guid ReferralId { get; set; }
        public Guid ReferrerId { get; set; }
        public Guid RefereeId { get; set; }
        public decimal CommissionRate { get; set; } = 1.0m;
        public decimal TotalReferralEarnings { get; set; }
        public int ReferralCount { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public virtual User Referrer { get; set; }
        public virtual User Referee { get; set; }
    }
}
