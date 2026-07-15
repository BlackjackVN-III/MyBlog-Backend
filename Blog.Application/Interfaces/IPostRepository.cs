using Blog.Application.DTOs.Blog;
using Blog.Application.Queries;
using Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Application.Interfaces
{
    public interface IPostRepository
    {
        Task<List<BlogPost>> GetAllPostsAsync(QueryObject query);
        Task<BlogPost?> GetBlogByIdAsync(Guid id);
        Task<BlogPost> CreateBlogAsync(BlogPost blogPost);
        Task<BlogPost?> UpdateBlogPostAsync(BlogPost blogPost, Guid id);
        Task DeleteBlogPostAsync(Guid id);
    }
}
