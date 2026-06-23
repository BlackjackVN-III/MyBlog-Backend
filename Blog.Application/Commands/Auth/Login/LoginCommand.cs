using Blog.Application.DTOs.Auth;
using Blog.Application.Interfaces;
using MediatR;

namespace Blog.Application.Commands.Auth.Login
{
 
    public record LoginCommand(LoginRequestDto Dto) : IRequest<AuthResultDto>;

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LoginAsync(request.Dto);
        }
    }
}
