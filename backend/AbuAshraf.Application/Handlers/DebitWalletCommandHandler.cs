using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AbuAshraf.Application.Commands;
using AbuAshraf.Application.Services;

namespace AbuAshraf.Application.Handlers
{
    /// <summary>
    /// Handler for wallet debit command
    /// </summary>
    public class DebitWalletCommandHandler : IRequestHandler<DebitWalletCommand, bool>
    {
        private readonly IWalletService _walletService;

        public DebitWalletCommandHandler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<bool> Handle(DebitWalletCommand request, CancellationToken cancellationToken)
        {
            return await _walletService.DebitWalletAsync(request.UserId, request.Amount, request.TransactionType, request.Description);
        }
    }
}
