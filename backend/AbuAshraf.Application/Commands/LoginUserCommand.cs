using MediatR;
using AbuAshraf.Application.DTOs;

namespace AbuAshraf.Application.Commands
{
    /// <summary>
    /// MediatR command for user login
    /// </summary>
    public class LoginUserCommand : IRequest<AuthResponseDto>
    {
        public LoginUserDto LoginDto { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        public LoginUserCommand(LoginUserDto dto, string ipAddress, string userAgent)
        {
            LoginDto = dto;
            IpAddress = ipAddress;
            UserAgent = userAgent;
        }
    }
}
