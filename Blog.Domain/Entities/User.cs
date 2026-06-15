using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Domain.Entities
{
    public  class User 
    {

        public Guid  Id { get; set; }
        public string  Username { get; set; } = string.Empty;   
        public string Email { get; set; } = string.Empty;
        public string Bio {  get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;

        public ICollection<BlogPost> Posts { get; set; } = new List<BlogPost>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}
