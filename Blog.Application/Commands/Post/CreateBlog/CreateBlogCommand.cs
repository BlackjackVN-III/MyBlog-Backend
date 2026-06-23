using Blog.Application.DTOs.Blog;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Commands.Post.CreateBlog
{
    public record CreateBlogCommand(CreateBlogRequestDto Dto) : IRequest<Guid>;

    public class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand, Guid>
    {
        private readonly IPostRepository _postRepository;
        private readonly IAppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateBlogCommandHandler(IPostRepository postRepository, IAppDbContext context, ICurrentUserService currentUserService)
        {
            _postRepository = postRepository;
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
        {
            var blog = request.Dto.toBlogFromCreateDto();
            blog.Id = Guid.NewGuid();
            blog.UserId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
            await _postRepository.CreateBlogAsync(blog);
            await _context.SaveChangesAsync(cancellationToken);
            return blog.Id;
        }
    }


}
