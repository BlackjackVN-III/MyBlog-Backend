using Blog.Application.DTOs.Blog;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using Blog.Domain.Entities;
using Blog.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Blog.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;
        public PostRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<BlogPost> CreateBlogAsync(BlogPost blogPost)
        {
            await _context.Blogs.AddAsync(blogPost);
            return blogPost;
        }

        public async Task DeleteBlogPostAsync(Guid id)
        {

            var deleteBlog = await GetBlogByIdAsync(id);
            if (deleteBlog != null)
            {
                _context.Blogs.Remove(deleteBlog);
                
            } else {
                throw new ArgumentNullException(nameof(deleteBlog));
            }
            
        }

        public async Task<List<BlogPost>> GetAllPostsAsync()
        {
            return await _context.Blogs.Include(x => x.Author).ToListAsync();
        }

        public async Task<BlogPost?> GetBlogByIdAsync(Guid id)
        {
            return await _context.Blogs.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost?> UpdateBlogPostAsync(BlogPost blogPost, Guid id)
        {
           var updateBlog = await GetBlogByIdAsync(id);
            if (updateBlog == null)
            {
                throw new Exception("Blog post not found");
            }
            updateBlog.Title = blogPost.Title;
            updateBlog.Slug = blogPost.Slug;
            updateBlog.Summary = blogPost.Summary;
            updateBlog.Content = blogPost.Content;
            updateBlog.UpdateOn = DateTime.Now;

            return updateBlog;
        }
    }
}
