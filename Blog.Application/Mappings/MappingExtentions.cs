using Blog.Application.DTOs.Blog;
using Blog.Application.DTOs.User;
using Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blog.Application.Mappings
{
    public static class MappingExtentions
    {
        public static BlogDto toBlogDto(this BlogPost blogPost)
        {

            return new BlogDto
            {
                Id = blogPost.Id, // Bê nguyên Id từ Entity sang DTO
                Title = blogPost.Title,
                Slug = blogPost.Slug,
                Summary = blogPost.Summary,
                Content = blogPost.Content,
                // Nếu có Include thêm thông tin User thì map luôn
                Author = blogPost.Author != null ? new UserDto
                {
                    Id = blogPost.Author.Id,
                    Username = blogPost.Author.Username
                } : null

            };
        }
        public static BlogPost toBlogFromCreateDto(this CreateBlogRequestDto CreateModel)
        {
            return new BlogPost
            {
                Title = CreateModel.Title,
                Slug = CreateModel.Slug,
                Summary = CreateModel.Summary,
                Content = CreateModel.Content
            };
        }


        
}
}
