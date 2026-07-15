using Blog.Application.Common;
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
        private readonly ICacheService _cacheService;

        public CreateBlogCommandHandler(IPostRepository postRepository, IAppDbContext context, ICurrentUserService currentUserService, ICacheService cacheService)
        {
            _postRepository = postRepository;
            _context = context;
            _currentUserService = currentUserService;
            _cacheService = cacheService;
        }

        public async Task<Guid> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
        {
            var blog = request.Dto.toBlogFromCreateDto();
            blog.Id = Guid.NewGuid();
            blog.UserId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

            if (blog.PostTags != null)
            {
                foreach (var pt in blog.PostTags)
                {
                    pt.PostId = blog.Id;
                }
            }
            await _postRepository.CreateBlogAsync(blog);
            await _context.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync(CacheKey.AllBlogs);

            return blog.Id;
        }
    }


}
