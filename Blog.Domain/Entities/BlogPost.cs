using Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Domain.Entities
{
    public class BlogPost
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreateOn { get; set; } = DateTime.Now;
        public DateTime? UpdateOn { get; set; }
        public Guid UserId { get; set; }
        public User Author { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();







    }
}
