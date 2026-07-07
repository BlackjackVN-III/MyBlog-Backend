using Blog.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Commands.Post.DeleteBlog
{
    public record DeleteBlogCommand(Guid id) : IRequest;
    public class DeleteBlogCommandHandler : IRequestHandler<DeleteBlogCommand>
    {
        private readonly IAppDbContext _context;
        private readonly IPostRepository _postRepository;
        private readonly ICurrentUserService _currentUserService;
        public DeleteBlogCommandHandler(IAppDbContext context, IPostRepository postRepository, ICurrentUserService currentUserService)
        {
            _context = context;
            _postRepository = postRepository;
            _currentUserService = currentUserService;
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
        }



    }
}

