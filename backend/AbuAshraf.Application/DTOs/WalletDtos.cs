using System;
using System.Collections.Generic;

namespace AbuAshraf.Application.DTOs
{
    /// <summary>
    /// DTO for wallet information
    /// </summary>
    public class WalletDto
    {
        public Guid WalletId { get; set; }
        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal HoldAmount { get; set; }
        public string Currency { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    /// <summary>
    /// DTO for wallet funding
    /// </summary>
    public class FundWalletDto
    {
        public decimal Amount { get; set; }
        public string PaymentGateway { get; set; } // Paystack, Flutterwave
    }

    /// <summary>
    /// DTO for wallet transaction
    /// </summary>
    public class WalletTransactionDto
    {
        public Guid TransactionId { get; set; }
        public string ReferenceNumber { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// DTO for transaction history
    /// </summary>
    public class TransactionHistoryDto
    {
        public List<WalletTransactionDto> Transactions { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    /// <summary>
    /// DTO for wallet dashboard summary
    /// </summary>
    public class WalletDashboardDto
    {
        public decimal Balance { get; set; }
        public decimal AvailableBalance { get; set; }
        public int TotalPurchases { get; set; }
        public decimal TotalSpending { get; set; }
        public decimal ReferralEarnings { get; set; }
    }
}
