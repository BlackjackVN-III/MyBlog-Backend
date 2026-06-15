using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Domain.Entities
{
    public class Tag
    {
        public  Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; } // hex color cho UI

        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();

    }
}
