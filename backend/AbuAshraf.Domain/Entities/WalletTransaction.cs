using System;

namespace AbuAshraf.Domain.Entities
{
    /// <summary>
    /// Wallet transaction entity tracking all wallet movements
    /// </summary>
    public class WalletTransaction
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public Guid WalletId { get; set; }
        public string ReferenceNumber { get; set; }
        public string TransactionType { get; set; } // Deposit, Airtime, Data, Cable, Electricity, Refund, Commission, Withdrawal
        public decimal Amount { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal NewBalance { get; set; }
        public string Status { get; set; } = "Pending";
        public string Description { get; set; }
        public string Metadata { get; set; } // JSON string
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Wallet Wallet { get; set; }
    }
}
