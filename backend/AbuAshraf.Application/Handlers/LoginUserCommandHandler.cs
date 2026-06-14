using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AbuAshraf.Application.Commands;
using AbuAshraf.Application.DTOs;
using AbuAshraf.Application.Services;

namespace AbuAshraf.Application.Handlers
{
    /// <summary>
    /// Handler for user login command
    /// </summary>
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponseDto>
    {
        private readonly IAuthenticationService _authService;

        public LoginUserCommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LoginAsync(request.LoginDto);
        }
    }
}
