namespace Blog.Application.DTOs.Auth
{
    /// <summary>
    /// DTO trả về cho client sau khi đăng ký hoặc đăng nhập thành công.
    /// Chứa thông tin user cơ bản + Access Token + Refresh Token.
    /// </summary>
    public class AuthResultDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Access Token (JWT) - sống ngắn (60 phút).
        /// Client gửi kèm trong Header "Authorization: Bearer {token}" mỗi lần gọi API.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Refresh Token - sống dài (7 ngày).
        /// Khi Access Token hết hạn, client gửi Refresh Token lên để nhận Access Token mới
        /// mà không cần đăng nhập lại.
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
    }
}
