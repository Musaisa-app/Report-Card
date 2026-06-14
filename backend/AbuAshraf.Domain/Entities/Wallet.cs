using System;
using System.Collections.Generic;

namespace AbuAshraf.Domain.Entities
{
    /// <summary>
    /// Wallet entity for user account balance management
    /// </summary>
    public class Wallet
    {
        public Guid WalletId { get; set; }
        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
        public decimal HoldAmount { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal DailyLimit { get; set; } = 1000000m;
        public decimal MonthlyLimit { get; set; } = 50000000m;
        public decimal DailySpent { get; set; }
        public decimal MonthlySpent { get; set; }
        public string Currency { get; set; } = "NGN";
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    }
}
