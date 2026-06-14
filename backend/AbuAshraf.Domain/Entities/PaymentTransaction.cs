using System;

namespace AbuAshraf.Domain.Entities
{
    /// <summary>
    /// Payment transaction entity for payment gateway integration
    /// </summary>
    public class PaymentTransaction
    {
        public Guid PaymentId { get; set; }
        public Guid? WalletTransactionId { get; set; }
        public Guid UserId { get; set; }
        public string Gateway { get; set; } // Paystack, Flutterwave, Monnify
        public string GatewayReference { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";
        public string GatewayResponse { get; set; } // JSON string
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual WalletTransaction WalletTransaction { get; set; }
    }
}
