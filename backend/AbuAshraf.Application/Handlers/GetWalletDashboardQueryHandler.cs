using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AbuAshraf.Application.DTOs;
using AbuAshraf.Application.Queries;
using AbuAshraf.Application.Services;

namespace AbuAshraf.Application.Handlers
{
    /// <summary>
    /// Handler for wallet dashboard query
    /// </summary>
    public class GetWalletDashboardQueryHandler : IRequestHandler<GetWalletDashboardQuery, WalletDashboardDto>
    {
        private readonly IWalletService _walletService;

        public GetWalletDashboardQueryHandler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<WalletDashboardDto> Handle(GetWalletDashboardQuery request, CancellationToken cancellationToken)
        {
            return await _walletService.GetDashboardSummaryAsync(request.UserId);
        }
    }
}
