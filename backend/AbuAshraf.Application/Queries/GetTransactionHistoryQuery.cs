using System;
using System.Threading.Tasks;
using MediatR;
using AbuAshraf.Application.DTOs;

namespace AbuAshraf.Application.Queries
{
    /// <summary>
    /// MediatR query for transaction history
    /// </summary>
    public class GetTransactionHistoryQuery : IRequest<TransactionHistoryDto>
    {
        public Guid UserId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetTransactionHistoryQuery(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            UserId = userId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
