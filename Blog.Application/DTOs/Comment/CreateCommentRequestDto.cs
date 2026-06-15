using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Blog.Application.DTOs.Comment
{
    public class CreateCommentRequestDto
    {
        [Required(ErrorMessage = "Mã bài viết là bắt buộc.")]
        public Guid PostId { get; set; }

        [Required(ErrorMessage = "Nội dung bình luận không được để trống.")]
        [MaxLength(1000, ErrorMessage = "Bình luận tối đa 1000 ký tự.")]
        public string Content { get; set; } = string.Empty;

        public Guid? ParentCommentId { get; set; } // Null nếu là bình luận gốc, có giá trị nếu là Reply
    }
}
