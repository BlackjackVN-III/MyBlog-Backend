using Blog.Application.DTOs.Blog;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Queries.GetBlog
{
    public record GetAllBlogsQuery : IRequest<List<BlogDto>>;

    public class GetAllBlogsQueryHandler : IRequestHandler<GetAllBlogsQuery,List<BlogDto>>
    {
        private readonly IPostRepository _postRepository;
        public GetAllBlogsQueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<List<BlogDto>> Handle(GetAllBlogsQuery request, CancellationToken cancellationToken)
        {
             var getBlogs = await _postRepository.GetAllPostsAsync();
             return getBlogs.Select(x => x.toBlogDto()).ToList();
        }


    }


}
