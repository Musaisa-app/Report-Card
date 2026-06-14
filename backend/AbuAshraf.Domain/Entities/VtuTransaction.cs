using System;

namespace AbuAshraf.Domain.Entities
{
    /// <summary>
    /// VTU transaction entity for airtime, data, cable, and electricity purchases
    /// </summary>
    public class VtuTransaction
    {
        public Guid VtuTransactionId { get; set; }
        public Guid? WalletTransactionId { get; set; }
        public Guid UserId { get; set; }
        public string ServiceType { get; set; } // Airtime, Data, Cable, Electricity
        public string Network { get; set; } // MTN, Airtel, Glo, 9mobile
        public string PhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public string VtuProvider { get; set; }
        public string ProviderReference { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal ApiCost { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Profit { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual WalletTransaction WalletTransaction { get; set; }
    }
}
