using Blog.Application.DTOs.Auth;
using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Blog.Infrastructure.Data;
using Blog.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Blog.Infrastructure.Service
{

    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(UserManager<AppUser> userManager,ITokenService tokenService,AppDbContext context,IConfiguration config)
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
                Id = appUser.Id,
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

        public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            // 1. Trích xuất claims principal từ access token đã hết hạn
            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
            if (principal == null)
            {
                throw new Exception("Access Token không hợp lệ.");
            }

            // 2. Tìm UserId từ claim NameIdentifier / Sub
            var userIdClaim = principal.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new Exception("Token thiếu thông tin định danh.");
            }

            // 3. Tìm user trong DB
            var appUser = await _userManager.FindByIdAsync(userId.ToString());
            if (appUser == null || appUser.RefreshToken != dto.RefreshToken || appUser.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                throw new Exception("Refresh Token không hợp lệ hoặc đã hết hạn.");
            }

            // 4. Sinh Access Token mới và Refresh Token mới
            var roles = await _userManager.GetRolesAsync(appUser);
            var newAccessToken = _tokenService.CreateToken(appUser.Id, appUser.Email!, appUser.UserName!, roles);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // 5. Cập nhật Refresh Token mới vào DB
            appUser.RefreshToken = newRefreshToken;
            appUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:RefreshTokenExpirationInDays"]!));
            await _userManager.UpdateAsync(appUser);

            return new AuthResultDto
            {
                UserName = appUser.UserName!,
                Email = appUser.Email!,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            var appUser = await _userManager.FindByIdAsync(userId.ToString());
            if (appUser == null)
            {
                return false;
            }

            // Hủy Refresh Token bằng cách gán null trong DB
            appUser.RefreshToken = null;
            appUser.RefreshTokenExpiry = null;
            var result = await _userManager.UpdateAsync(appUser);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var appUser = await _userManager.FindByIdAsync(userId.ToString());
            if (appUser == null)
            {
                return false;
            }

            var result = await _userManager.ChangePasswordAsync(appUser, currentPassword, newPassword);
            return result.Succeeded;
        }
    }
}
