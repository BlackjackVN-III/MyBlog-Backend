using Blog.Application.DTOs.Blog;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using Blog.Application.Queries;
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

        public async Task<List<BlogPost>> GetAllPostsAsync(QueryObject query)
        {
            var posts = _context.Blogs
                .Include(x => x.Author)
                .Include(x => x.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .AsQueryable();

            // 1. Tìm kiếm theo Tiêu đề hoặc Nội dung
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                posts = posts.Where(p => p.Title.Contains(query.Search) || p.Content.Contains(query.Search));
            }

            // 2. Lọc theo Tag Slug
            if (!string.IsNullOrWhiteSpace(query.TagSlug))
            {
                posts = posts.Where(p => p.PostTags.Any(pt => pt.Tag.Slug == query.TagSlug));
            }

            // 3. Sắp xếp (Sorting)
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    posts = query.isDecsending 
                        ? posts.OrderByDescending(p => p.Title) 
                        : posts.OrderBy(p => p.Title);
                }
            }
            else
            {
                // Mặc định luôn sắp xếp bài viết mới nhất lên trước
                posts = posts.OrderByDescending(p => p.CreateOn);
            }

            // 4. Phân trang (Pagination)
            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await posts.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<BlogPost?> GetBlogByIdAsync(Guid id)
        {
            return await _context.Blogs
                .Include(x => x.Author)
                .Include(x => x.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost?> UpdateBlogPostAsync(BlogPost blogPost, Guid id)
        {
            var updateBlog = await _context.Blogs
                .Include(x => x.PostTags)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (updateBlog == null)
            {
                throw new Exception("Blog post not found");
            }
            updateBlog.Title = blogPost.Title;
            updateBlog.Slug = blogPost.Slug;
            updateBlog.Summary = blogPost.Summary;
            updateBlog.Content = blogPost.Content;
            updateBlog.UpdateOn = DateTime.Now;
            updateBlog.CoverImageUrl = blogPost.CoverImageUrl;

            // Cập nhật Many-to-Many PostTags
            updateBlog.PostTags.Clear();
            foreach (var pt in blogPost.PostTags)
            {
                pt.PostId = id;
                updateBlog.PostTags.Add(pt);
            }

            return updateBlog;
        }
    }
}
