using Blog.Application.Interfaces;
using MediatR;

namespace Blog.Application.Commands.Auth.Logout
{
    public record LogoutCommand : IRequest<bool>;
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;
        public LogoutCommandHandler(IAuthService authService, ICurrentUserService currentUserService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null || userId == Guid.Empty)
            {
                return false;
            }
            return await _authService.LogoutAsync(userId.Value);
        }
    }
}
