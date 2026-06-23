using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Domain.Entities
{
    public class Comment

    {
        public Guid Id { get; set; }
        public string  Title { get; set; } = string.Empty;
        public string  Content { get; set; } = string.Empty;
        public DateTime CreateOn { get; set; } = DateTime.Now;
        public Guid BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; } = null!;

        public Guid UserId { get; set; }
        public User Author { get; set; } = null!;

        // Self-referencing — nested comment
        public Guid? ParentId { get; set; }
        public Comment? Parent { get; set; } = null!;
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
