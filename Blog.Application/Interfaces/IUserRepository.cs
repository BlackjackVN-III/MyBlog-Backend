using Blog.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Blog.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task UpdateUserAsync(User user);
    }
}
