using Blog.Application.DTOs.Blog;
using Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Interfaces
{
    public interface IPostRepository
    {
        Task<List<BlogPost>> GetAllPostsAsync();
        Task<BlogPost?> GetBlogByIdAsync(Guid id);
        Task<BlogPost> CreateBlogAsync(BlogPost blogPost);
        Task<BlogPost?> UpdateBlogPostAsync(BlogPost blogPost, Guid id);
        Task DeleteBlogPostAsync(Guid id);
    }
}
