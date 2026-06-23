
using Microsoft.AspNetCore.Identity;

namespace Blog.Infrastructure.Identity
{
    public class AppUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Refresh Token hiện tại của user, được lưu trong DB.
        /// Khi client gửi refresh token lên, backend sẽ so sánh với giá trị này.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Thời điểm Refresh Token hết hạn.
        /// Nếu quá thời gian này, user phải đăng nhập lại.
        /// </summary>
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
