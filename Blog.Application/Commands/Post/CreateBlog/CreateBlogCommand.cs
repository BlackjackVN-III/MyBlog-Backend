using Blog.Application.DTOs.Blog;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Commands.Post.CreateBlog
{
    public record CreateBlogCommand(CreateBlogRequestDto Dto) : IRequest<BlogDto>;

    public class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand, BlogDto>
    {
        private readonly IPostRepository _postRepository;
        public CreateBlogCommandHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        

        public async Task<BlogDto> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
        {
            var blog = request.Dto.toBlogFromCreateDto();
            var result = await _postRepository.CreateBlogAsync(blog);
            return result.toBlogDto();
        }
    }


}
