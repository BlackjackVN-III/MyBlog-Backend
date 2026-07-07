using Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Application.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetCommentsByPostIdAsync(Guid postId);
        Task<Comment?> GetCommentByIdAsync(Guid id);
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<Comment?> DeleteCommentAsync(Guid id);
    }
}
