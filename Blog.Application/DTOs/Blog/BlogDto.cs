using Blog.Application.DTOs.Tag;
using Blog.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.DTOs.Blog
{
    public class BlogDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public UserDto? Author { get; set; }
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}
