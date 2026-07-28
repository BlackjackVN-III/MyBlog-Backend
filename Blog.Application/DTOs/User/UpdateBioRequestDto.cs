using System.ComponentModel.DataAnnotations;

namespace Blog.Application.DTOs.User
{
    public class UpdateBioRequestDto
    {
        [MaxLength(500, ErrorMessage = "Giới thiệu bản thân không được vượt quá 500 ký tự.")]
        public string Bio { get; set; } = string.Empty;
    }
}
