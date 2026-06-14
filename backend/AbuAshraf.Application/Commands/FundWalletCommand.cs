using System;
using System.Threading.Tasks;
using MediatR;
using AbuAshraf.Application.DTOs;

namespace AbuAshraf.Application.Commands
{
    /// <summary>
    /// MediatR command for wallet funding
    /// </summary>
    public class FundWalletCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentGateway { get; set; }

        public FundWalletCommand(Guid userId, decimal amount, string paymentGateway)
        {
            UserId = userId;
            Amount = amount;
            PaymentGateway = paymentGateway;
        }
    }
}
