using Blog.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Infrastructure.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            // Tạo khóa đối xứng từ SigningKey trong appsettings.json
            // Khóa này dùng để KÝ (sign) và XÁC THỰC (validate) JWT Token
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
        }

        /// <summary>
        /// Sinh Access Token (JWT).
        /// Token này chứa các Claims (thông tin user) và có thời hạn ngắn.
        /// </summary>
        public string CreateToken(Guid userId, string email, string userName, IList<string> roles)
        {
            // 1. Tạo danh sách Claims - đây là các thông tin được nhúng bên trong JWT
            //    Backend sẽ đọc các claims này từ HttpContext.User để biết ai đang gọi API
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),   // Subject = UserId
                new Claim(JwtRegisteredClaimNames.Email, email),              // Email
                new Claim(JwtRegisteredClaimNames.GivenName, userName),       // Username
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JWT ID (unique per token)
            };

            // 2. Thêm các Role vào Claims
            //    Ví dụ: nếu user có role "Admin", ta thêm claim "role" = "Admin"
            //    Middleware sẽ dùng claim này khi kiểm tra [Authorize(Roles = "Admin")]
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 3. Tạo signing credentials - xác định thuật toán mã hóa chữ ký
            //    HmacSha512Signature = thuật toán hash mạnh, dùng khóa đối xứng
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // 4. Mô tả token: ai phát hành, dành cho ai, hết hạn khi nào
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(
                    double.Parse(_config["JWT:TokenExpirationInMinutes"]!)),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            // 5. Sinh token từ descriptor và chuyển thành chuỗi string
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Sinh Refresh Token - chỉ là chuỗi random 64 bytes, không phải JWT.
        /// Token này sẽ được lưu trong DB (bảng AppUser) để đối chiếu khi client gửi lên.
        /// </summary>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
