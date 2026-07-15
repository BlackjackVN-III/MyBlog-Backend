using Blog.Application.Interfaces;
using MediatR;

namespace Blog.Application.Commands.Auth.Logout
{
    public record LogoutCommand : IRequest<bool>;
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICacheService _cacheService;
        public LogoutCommandHandler(IAuthService authService, ICurrentUserService currentUserService, ICacheService cacheService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
            _cacheService = cacheService;
        }
        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null || userId == Guid.Empty)
            {
                return false;
            }
            // Vô hiệu hóa Refresh Token dưới DB
            var success = await _authService.LogoutAsync(userId.Value);
            if (!success)
            {
                return false;
            }
            // Đưa Access Token hiện tại vào Blacklist của Redis
            var token = _currentUserService.Token;
            if (!string.IsNullOrEmpty(token))
            {
                var blacklistKey = $"blacklist:{token}";
                await _cacheService.SetAsync(blacklistKey, "revoked", TimeSpan.FromMinutes(60));
            }
            return true;
        }
    }
}
