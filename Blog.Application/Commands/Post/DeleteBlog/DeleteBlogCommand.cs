using Blog.Application.Common;
using Blog.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Application.Commands.Post.DeleteBlog
{
    public record DeleteBlogCommand(Guid id) : IRequest;

    public class DeleteBlogCommandHandler : IRequestHandler<DeleteBlogCommand>
    {
        private readonly IAppDbContext _context;
        private readonly IPostRepository _postRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICacheService _cacheService;

        public DeleteBlogCommandHandler(IAppDbContext context, IPostRepository postRepository, ICurrentUserService currentUserService, ICacheService cacheService)
        {
            _context = context;
            _postRepository = postRepository;
            _currentUserService = currentUserService;
            _cacheService = cacheService;
        }

        public async Task Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
        {
            var existedBlog = await _postRepository.GetBlogByIdAsync(request.id);
            if (existedBlog == null)
            {
                throw new Exception("Không tìm thấy bài viết");
            }
            var currentUserId = _currentUserService.UserId;
            var isAdmin = _currentUserService.IsInRole("Admin");
            if (existedBlog.UserId != currentUserId && !isAdmin)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa bài viết này.");
            }

            await _postRepository.DeleteBlogPostAsync(request.id);
            await _context.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync(CacheKey.AllBlogs);
        }
    }
}
