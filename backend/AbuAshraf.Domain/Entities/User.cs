using System;
using System.Collections.Generic;

namespace AbuAshraf.Domain.Entities
{
    /// <summary>
    /// User entity representing system users (Admin, Agent, Customer)
    /// </summary>
    public class User
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public string ReferralCode { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public string Status { get; set; } = "Active";
        public bool AccountLocked { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Navigation properties
        public virtual Role Role { get; set; }
        public virtual Wallet Wallet { get; set; }
        public virtual ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
        public virtual ICollection<VtuTransaction> VtuTransactions { get; set; } = new List<VtuTransaction>();
        public virtual Agent Agent { get; set; }
        public virtual ICollection<Referral> ReferralsAsReferrer { get; set; } = new List<Referral>();
        public virtual ICollection<Referral> ReferralsAsReferee { get; set; } = new List<Referral>();
    }
}
