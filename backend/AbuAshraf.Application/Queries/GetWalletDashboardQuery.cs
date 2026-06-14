using System;
using System.Threading.Tasks;
using MediatR;
using AbuAshraf.Application.DTOs;

namespace AbuAshraf.Application.Queries
{
    /// <summary>
    /// MediatR query for wallet dashboard
    /// </summary>
    public class GetWalletDashboardQuery : IRequest<WalletDashboardDto>
    {
        public Guid UserId { get; set; }

        public GetWalletDashboardQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
