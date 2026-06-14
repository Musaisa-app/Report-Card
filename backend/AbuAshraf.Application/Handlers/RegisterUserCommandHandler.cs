using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AbuAshraf.Application.Commands;
using AbuAshraf.Application.DTOs;
using AbuAshraf.Application.Services;
using AbuAshraf.Domain.Entities;

namespace AbuAshraf.Application.Handlers
{
    /// <summary>
    /// Handler for user registration command
    /// </summary>
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
    {
        private readonly IAuthenticationService _authService;

        public RegisterUserCommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RegisterAsync(request.RegisterDto);
        }
    }
}
