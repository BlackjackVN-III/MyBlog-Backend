using Blog.Application.DTOs.Blog;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Commands.Post.UpdateBlog
{
    public record UpdateBlogCommand(UpdateBlogRequestDto dto, Guid id) : IRequest<BlogDto>;

    public class UpdateBlogCommandHandler : IRequestHandler<UpdateBlogCommand, BlogDto>
    {
        private readonly IAppDbContext _context;
        private readonly IPostRepository _postRepository;

        public UpdateBlogCommandHandler(IAppDbContext appDbContext, IPostRepository postRepository)
        {
            _context = appDbContext;
            _postRepository = postRepository;
        }

        public async Task<BlogDto> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
        {
            var blog = request.dto.toBlogFromUpdateDto();
            var result = await _postRepository.UpdateBlogPostAsync(blog, request.id);
            if (result == null) {
                throw new Exception("Không tìm thấy bài viết");
            }
            await _context.SaveChangesAsync(cancellationToken);
            return result.toBlogDto();

        }
    }
    }



