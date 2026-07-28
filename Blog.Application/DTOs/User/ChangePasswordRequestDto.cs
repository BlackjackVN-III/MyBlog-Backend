using System.ComponentModel.DataAnnotations;

namespace Blog.Application.DTOs.User
{
    public class ChangePasswordRequestDto
    {
        [Required(ErrorMessage = "Mật khẩu cũ không được để trống.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [MinLength(12, ErrorMessage = "Mật khẩu mới phải có ít nhất 12 ký tự.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
