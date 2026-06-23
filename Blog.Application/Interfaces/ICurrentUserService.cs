namespace Blog.Application.Interfaces
{
    /// <summary>
    /// Interface trừu tượng để lấy thông tin người dùng hiện tại.
    /// Nằm ở Application layer → Handler inject được mà không phụ thuộc ASP.NET Core.
    /// Implementation thật sẽ nằm ở Infrastructure layer (đọc từ HttpContext).
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Id của Domain User đang đăng nhập. Null nếu chưa đăng nhập.
        /// </summary>
        Guid? UserId { get; }

        string? UserName { get; }
    }
}
