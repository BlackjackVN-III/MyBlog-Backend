using System.Security.Claims;
using Blog.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Blog.Infrastructure.Service
{
    /// <summary>
    /// Đọc thông tin người dùng hiện tại từ JWT Token trong HttpContext.
    /// Middleware JWT Bearer đã giải mã token và gắn Claims vào HttpContext.User,
    /// service này chỉ việc đọc ra.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                // Claim "sub" trong JWT chứa UserId (do TokenService đã set ở CreateToken)
                // JwtRegisteredClaimNames.Sub ("sub") được ASP.NET Core tự động map
                // sang ClaimTypes.NameIdentifier trong một số cấu hình
                var userIdClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

                return Guid.TryParse(userIdClaim, out var parsed) ? parsed : null;
            }
        }

        public string? UserName =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value;
    }
}
