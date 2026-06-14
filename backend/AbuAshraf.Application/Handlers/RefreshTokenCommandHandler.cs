using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AbuAshraf.Application.Commands;
using AbuAshraf.Application.DTOs;
using AbuAshraf.Application.Services;

namespace AbuAshraf.Application.Handlers
{
    /// <summary>
    /// Handler for token refresh command
    /// </summary>
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponseDto>
    {
        private readonly IAuthenticationService _authService;

        public RefreshTokenCommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<TokenResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RefreshTokenAsync(request.RefreshToken, request.IpAddress);
        }
    }
}
