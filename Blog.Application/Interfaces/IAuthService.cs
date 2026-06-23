using Blog.Application.DTOs.Auth;

namespace Blog.Application.Interfaces
{
    /// <summary>
    /// Interface trừu tượng hóa toàn bộ thao tác Authentication.
    /// Tầng Application chỉ biết interface này, không biết gì về UserManager hay AppUser.
    /// Implementation thực sự nằm ở tầng Infrastructure (AuthService.cs).
    /// </summary>
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResultDto> LoginAsync(LoginRequestDto dto);
    }
}
