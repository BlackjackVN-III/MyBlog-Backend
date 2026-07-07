using Blog.Application.DTOs.Auth;
using Blog.Application.Interfaces;
using MediatR;

namespace Blog.Application.Commands.Auth.RefreshToken
{
    public record RefreshTokenCommand(RefreshTokenRequestDto Dto) : IRequest<AuthResultDto>;
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResultDto>
    {
        private readonly IAuthService _authService;
        public RefreshTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<AuthResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RefreshTokenAsync(request.Dto);
        }
    }
}
