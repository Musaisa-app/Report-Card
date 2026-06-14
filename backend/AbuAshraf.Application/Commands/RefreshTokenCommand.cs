using MediatR;
using AbuAshraf.Application.DTOs;

namespace AbuAshraf.Application.Commands
{
    /// <summary>
    /// MediatR command for token refresh
    /// </summary>
    public class RefreshTokenCommand : IRequest<TokenResponseDto>
    {
        public string RefreshToken { get; set; }
        public string IpAddress { get; set; }

        public RefreshTokenCommand(string refreshToken, string ipAddress)
        {
            RefreshToken = refreshToken;
            IpAddress = ipAddress;
        }
    }
}
