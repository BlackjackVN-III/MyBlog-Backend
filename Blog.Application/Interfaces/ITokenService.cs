namespace Blog.Application.Interfaces
{
    /// <summary>
    /// Interface cho việc sinh JWT Token.
    /// Nhận các tham số đơn giản (không dùng AppUser) để giữ tầng Application 
    /// không phụ thuộc vào Identity/Infrastructure.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Sinh Access Token (JWT) chứa thông tin user và roles.
        /// </summary>
        string CreateToken(Guid userId, string email, string userName, IList<string> roles);

        /// <summary>
        /// Sinh Refresh Token (chuỗi ngẫu nhiên, không phải JWT).
        /// Client dùng token này để xin Access Token mới khi hết hạn.
        /// </summary>
        string GenerateRefreshToken();
    }
}
