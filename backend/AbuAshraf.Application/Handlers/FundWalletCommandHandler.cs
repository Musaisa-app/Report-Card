using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AbuAshraf.Application.Commands;
using AbuAshraf.Application.Services;

namespace AbuAshraf.Application.Handlers
{
    /// <summary>
    /// Handler for wallet funding command
    /// </summary>
    public class FundWalletCommandHandler : IRequestHandler<FundWalletCommand, bool>
    {
        private readonly IWalletService _walletService;

        public FundWalletCommandHandler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<bool> Handle(FundWalletCommand request, CancellationToken cancellationToken)
        {
            return await _walletService.FundWalletAsync(request.UserId, request.Amount, $"Deposit_{request.PaymentGateway}");
        }
    }
}
