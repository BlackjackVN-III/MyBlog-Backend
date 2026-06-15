using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.DTOs.Tag
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }
}
