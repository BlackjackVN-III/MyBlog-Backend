using Blog.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Blog.Application.DTOs.Blog
{
    public class CreateBlogRequestDto
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        [MaxLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Đường dẫn (Slug) là bắt buộc.")]
        [MaxLength(255, ErrorMessage = "Slug không được vượt quá 255 ký tự.")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug chỉ được chứa chữ cái thường, số và dấu gạch ngang.")]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Tóm tắt không được vượt quá 500 ký tự.")]
        public string Summary { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung bài viết không được để trống.")]
        public string Content { get; set; } = string.Empty;

        // Nhận danh sách ID của các Tag mà người dùng chọn khi tạo bài
        public List<Guid> TagIds { get; set; } = new List<Guid>();


    }
}
