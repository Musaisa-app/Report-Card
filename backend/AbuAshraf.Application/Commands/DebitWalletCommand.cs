using System;
using System.Threading.Task;
using MediatR;

namespace AbuAshraf.Application.Commands
{
    /// <summary>
    /// MediatR command for wallet debit
    /// </summary>
    public class DebitWalletCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }

        public DebitWalletCommand(Guid userId, decimal amount, string transactionType, string description)
        {
            UserId = userId;
            Amount = amount;
            TransactionType = transactionType;
            Description = description;
        }
    }
}
