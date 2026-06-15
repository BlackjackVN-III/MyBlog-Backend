using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Blog.Application.DTOs.Tag
{
    public class CreateTagRequestDto
    {
        [Required(ErrorMessage = "Tên thẻ không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên thẻ không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-z0-9-]+$")]
        public string Slug { get; set; } = string.Empty;
    }
}
