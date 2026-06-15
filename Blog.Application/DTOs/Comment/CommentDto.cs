using Blog.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.DTOs.Comment
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public Guid PostId { get; set; }
        public UserDto User { get; set; } = null!;

        public Guid? ParentCommentId { get; set; }
    }
}
