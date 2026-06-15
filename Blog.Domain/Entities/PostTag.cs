using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Domain.Entities
{
    public class PostTag
    {
        public Guid PostId { get; set; }
        public BlogPost BlogPost { get; set; } = null!;

        public Guid TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}
