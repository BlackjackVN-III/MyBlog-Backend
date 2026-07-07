using System.Security.Claims;

namespace Blog.Application.Interfaces
{
    /// <summary>
    /// Interface cho việc sinh JWT Token.
    /// Nhận các tham số đơn giản (không dùng AppUser) để giữ tầng Application 
    /// </summary>
    public interface ITokenService
    {

        string CreateToken(Guid userId, string email, string userName, IList<string> roles);

        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
