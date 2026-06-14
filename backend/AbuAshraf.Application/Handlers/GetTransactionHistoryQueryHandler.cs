using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AbuAshraf.Application.DTOs;
using AbuAshraf.Application.Queries;
using AbuAshraf.Application.Services;

namespace AbuAshraf.Application.Handlers
{
    /// <summary>
    /// Handler for transaction history query
    /// </summary>
    public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, TransactionHistoryDto>
    {
        private readonly IWalletService _walletService;

        public GetTransactionHistoryQueryHandler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<TransactionHistoryDto> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            return await _walletService.GetTransactionHistoryAsync(request.UserId, request.PageNumber, request.PageSize);
        }
    }
}
