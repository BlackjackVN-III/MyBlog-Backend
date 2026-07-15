using Blog.Application.DTOs.Blog;
using Blog.Application.DTOs.Comment;
using Blog.Application.DTOs.Tag;
using Blog.Application.DTOs.User;
using Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blog.Application.Mappings
{
    public static class MappingExtentions
    {
        public static BlogDto toBlogDto(this BlogPost blogPost)
        {
            return new BlogDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Slug = blogPost.Slug,
                Summary = blogPost.Summary,
                Content = blogPost.Content,
                CreatedAt = blogPost.CreateOn,
                CoverImageUrl = blogPost.CoverImageUrl,
                Author = blogPost.Author != null ? new UserDto
                {
                    Id = blogPost.Author.Id,
                    Username = blogPost.Author.Username
                } : null,
                Tags = blogPost.PostTags != null
                    ? blogPost.PostTags
                        .Where(pt => pt.Tag != null)
                        .Select(pt => pt.Tag.toTagDto())
                        .ToList()
                    : new List<TagDto>()
            };
        }

        public static BlogPost toBlogFromCreateDto(this CreateBlogRequestDto CreateModel)
        {
            return new BlogPost
            {
                Title = CreateModel.Title,
                Slug = CreateModel.Slug,
                Summary = CreateModel.Summary,
                Content = CreateModel.Content,
                CoverImageUrl = CreateModel.CoverImageUrl,
                PostTags = CreateModel.TagIds != null
                    ? CreateModel.TagIds.Select(tagId => new PostTag { TagId = tagId }).ToList()
                    : new List<PostTag>()
            };
        }

        public static BlogPost toBlogFromUpdateDto(this UpdateBlogRequestDto updateModel)
        {
            return new BlogPost
            {
                Title = updateModel.Title,
                Slug = updateModel.Slug,
                Summary = updateModel.Summary,
                Content = updateModel.Content,
                CoverImageUrl = updateModel.CoverImageUrl,
                PostTags = updateModel.TagIds != null
                    ? updateModel.TagIds.Select(tagId => new PostTag { TagId = tagId }).ToList()
                    : new List<PostTag>()
            };
        }

        public static TagDto toTagDto(this Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Slug = tag.Slug
            };
        }

        public static Tag toTagFromCreateDto(this CreateTagRequestDto createModel)
        {
            return new Tag
            {
                Name = createModel.Name,
                Slug = createModel.Slug
            };
        }

        public static CommentDto toCommentDto(this Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreateOn,
                PostId = comment.BlogPostId,
                ParentCommentId = comment.ParentId,
                User = comment.Author != null ? new UserDto
                {
                    Id = comment.Author.Id,
                    Username = comment.Author.Username
                } : null!
            };
        }

        public static Comment toCommentFromCreateDto(this CreateCommentRequestDto createModel)
        {
            return new Comment
            {
                Content = createModel.Content,
                BlogPostId = createModel.PostId,
                ParentId = createModel.ParentCommentId
            };
        }
    }
}
