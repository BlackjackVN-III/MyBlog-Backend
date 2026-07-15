using Blog.Application.Common;
using Blog.Application.DTOs.Blog;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using MediatR;

namespace Blog.Application.Commands.Post.UpdateBlog
{
    public record UpdateBlogCommand(UpdateBlogRequestDto dto, Guid id) : IRequest<BlogDto>;

    public class UpdateBlogCommandHandler : IRequestHandler<UpdateBlogCommand, BlogDto>
    {
        private readonly IAppDbContext _context;
        private readonly IPostRepository _postRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICacheService _cacheService;

        public UpdateBlogCommandHandler(IAppDbContext appDbContext, IPostRepository postRepository, ICurrentUserService currentUserService, ICacheService cacheService)
        {
            _context = appDbContext;
            _postRepository = postRepository;
            _currentUserService = currentUserService;
            _cacheService = cacheService;
        }

        public async Task<BlogDto> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
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

            var blog = request.dto.toBlogFromUpdateDto();
            var result = await _postRepository.UpdateBlogPostAsync(blog, request.id);
            await _context.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync(CacheKey.AllBlogs);

            return result.toBlogDto();
        }
    }
}





