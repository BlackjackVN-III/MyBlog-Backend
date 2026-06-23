using Blog.Application.DTOs.Auth;
using Blog.Application.Interfaces;
using MediatR;

namespace Blog.Application.Commands.Auth.Register
{

    public record RegisterCommand(RegisterRequestDto Dto) : IRequest<AuthResultDto>;

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResultDto>
    {
        private readonly IAuthService _authService;

        public RegisterCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RegisterAsync(request.Dto);
        }
    }
}
