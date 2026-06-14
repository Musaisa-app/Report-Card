using MediatR;
using AbuAshraf.Application.DTOs;

namespace AbuAshraf.Application.Commands
{
    /// <summary>
    /// MediatR command for user registration
    /// </summary>
    public class RegisterUserCommand : IRequest<AuthResponseDto>
    {
        public RegisterUserDto RegisterDto { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        public RegisterUserCommand(RegisterUserDto dto, string ipAddress, string userAgent)
        {
            RegisterDto = dto;
            IpAddress = ipAddress;
            UserAgent = userAgent;
        }
    }
}
