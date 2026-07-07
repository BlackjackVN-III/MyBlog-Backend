using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Blog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetCommentsByPostIdAsync(Guid postId)
        {
            return await _context.Comments
                .Include(c => c.Author)
                .Where(c => c.BlogPostId == postId)
                .OrderByDescending(c => c.CreateOn)
                .ToListAsync();
        }

        public async Task<Comment?> GetCommentByIdAsync(Guid id)
        {
            return await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.BlogPost)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            return comment;
        }

        public async Task<Comment?> DeleteCommentAsync(Guid id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null) return null;
            await DeleteRepliesRecursiveAsync(comment.Id);

            _context.Comments.Remove(comment);
            return comment;
        }

        private async Task DeleteRepliesRecursiveAsync(Guid parentId)
        {
            var replies = await _context.Comments.Where(c => c.ParentId == parentId).ToListAsync();
            foreach (var reply in replies)
            {
                await DeleteRepliesRecursiveAsync(reply.Id);
                _context.Comments.Remove(reply);
            }
        }
    }
}
