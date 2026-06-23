using Blog.Application.DTOs.Blog;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Queries.GetBlogById
{
    public record GetBlogByIdQuery(Guid Id) : IRequest<BlogDto>;

    public class GetBlogByIdQueryHandler : IRequestHandler<GetBlogByIdQuery, BlogDto>
    {
        private readonly IPostRepository _postRepository;
        public GetBlogByIdQueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<BlogDto> Handle(GetBlogByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _postRepository.GetBlogByIdAsync(request.Id);
            if (result == null)
            {
                /*throw new NotFoundException($"Không tìm thấy bài viết với Id {request.Id}");*/
                throw new Exception($"Không tìm thấy bài viết bạn tìm kiếm");
            }
            return result.toBlogDto();
        }
    }
}
