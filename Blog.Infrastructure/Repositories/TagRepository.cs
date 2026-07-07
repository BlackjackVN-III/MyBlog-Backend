using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Blog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<Tag?> GetTagByIdAsync(Guid id)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tag?> GetTagBySlugAsync(string slug)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Slug == slug);
        }

        public async Task<Tag> CreateTagAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
            return tag;
        }

        public async Task<Tag?> UpdateTagAsync(Tag tag, Guid id)
        {
            var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (existingTag == null) return null;

            existingTag.Name = tag.Name;
            existingTag.Slug = tag.Slug;
            existingTag.Description = tag.Description;
            existingTag.Color = tag.Color;

            return existingTag;
        }

        public async Task<Tag?> DeleteTagAsync(Guid id)
        {
            var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (existingTag == null) return null;

            _context.Tags.Remove(existingTag);
            return existingTag;
        }
    }
}
