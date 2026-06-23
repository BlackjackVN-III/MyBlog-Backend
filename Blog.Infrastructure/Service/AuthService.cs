using Blog.Application.DTOs.Auth;
using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Blog.Infrastructure.Data;
using Blog.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Blog.Infrastructure.Service
{
    /// <summary>
    /// Implementation của IAuthService - nằm ở tầng Infrastructure.
    /// Đây là nơi DUY NHẤT sử dụng UserManager và AppUser.
    /// Tầng Application (Handler) chỉ biết interface IAuthService.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(
            UserManager<AppUser> userManager,
            ITokenService tokenService,
            AppDbContext context,
            IConfiguration config)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
            _config = config;
        }

        /// <summary>
        /// Đăng ký tài khoản mới.
        /// Luồng: Tạo AppUser (Identity) → Gán Role → Tạo User (Domain) → Sinh JWT Token.
        /// </summary>
        public async Task<AuthResultDto> RegisterAsync(RegisterRequestDto dto)
        {
            // 1. Tạo AppUser cho Identity (quản lý đăng nhập, hash password)
            var appUser = new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            // 2. UserManager.CreateAsync sẽ:
            //    - Validate password theo rules đã cấu hình (>=12 ký tự, có chữ hoa, số, ký tự đặc biệt)
            //    - Hash password (không lưu plain text)
            //    - Lưu vào bảng AspNetUsers
            var createdUser = await _userManager.CreateAsync(appUser, dto.Password);

            if (!createdUser.Succeeded)
            {
                // Gom tất cả lỗi validation thành 1 chuỗi để trả về
                var errors = string.Join("; ", createdUser.Errors.Select(e => e.Description));
                throw new Exception($"Đăng ký thất bại: {errors}");
            }

            // 3. Gán Role mặc định "User" cho tài khoản mới
            //    Sau này Admin tạo tài khoản có thể gán role "Admin"
            await _userManager.AddToRoleAsync(appUser, "User");

            // 4. Tạo bản ghi User ở tầng Domain
            //    Bảng này liên kết 1-1 với AppUser qua cùng Id
            //    Dùng để lưu thông tin profile (Bio, Avatar) tách biệt khỏi Identity
            var domainUser = new User
            {
                Id = appUser.Id,       // Lấy Id từ AppUser (đã được Identity sinh ra)
                Username = dto.UserName,
                Email = dto.Email
            };

            await _context.DomainUsers.AddAsync(domainUser);
            await _context.SaveChangesAsync(CancellationToken.None);

            // 5. Sinh JWT Token
            var roles = await _userManager.GetRolesAsync(appUser);
            var token = _tokenService.CreateToken(appUser.Id, appUser.Email!, appUser.UserName!, roles);

            // 6. Sinh và lưu Refresh Token vào DB
            var refreshToken = _tokenService.GenerateRefreshToken();
            appUser.RefreshToken = refreshToken;
            appUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                int.Parse(_config["JWT:RefreshTokenExpirationInDays"]!));
            await _userManager.UpdateAsync(appUser);

            return new AuthResultDto
            {
                UserName = appUser.UserName!,
                Email = appUser.Email!,
                Token = token,
                RefreshToken = refreshToken
            };
        }

        /// <summary>
        /// Đăng nhập.
        /// Luồng: Tìm user → Kiểm tra password → Sinh JWT + Refresh Token.
        /// </summary>
        public async Task<AuthResultDto> LoginAsync(LoginRequestDto dto)
        {
            // 1. Tìm user theo UserName
            var appUser = await _userManager.FindByNameAsync(dto.UserName);
            if (appUser == null)
            {
                throw new Exception("Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            // 2. Kiểm tra password
            //    CheckPasswordAsync sẽ hash password người dùng nhập 
            //    rồi so sánh với hash đã lưu trong DB
            var isPasswordValid = await _userManager.CheckPasswordAsync(appUser, dto.Password);
            if (!isPasswordValid)
            {
                throw new Exception("Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            // 3. Sinh Access Token
            var roles = await _userManager.GetRolesAsync(appUser);
            var token = _tokenService.CreateToken(appUser.Id, appUser.Email!, appUser.UserName!, roles);

            // 4. Sinh và lưu Refresh Token mới
            var refreshToken = _tokenService.GenerateRefreshToken();
            appUser.RefreshToken = refreshToken;
            appUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                int.Parse(_config["JWT:RefreshTokenExpirationInDays"]!));
            await _userManager.UpdateAsync(appUser);

            return new AuthResultDto
            {
                UserName = appUser.UserName!,
                Email = appUser.Email!,
                Token = token,
                RefreshToken = refreshToken
            };
        }
    }
}
