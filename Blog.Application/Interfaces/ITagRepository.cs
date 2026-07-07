using Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Application.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllTagsAsync();
        Task<Tag?> GetTagByIdAsync(Guid id);
        Task<Tag?> GetTagBySlugAsync(string slug);
        Task<Tag> CreateTagAsync(Tag tag);
        Task<Tag?> UpdateTagAsync(Tag tag, Guid id);
        Task<Tag?> DeleteTagAsync(Guid id);
    }
}
