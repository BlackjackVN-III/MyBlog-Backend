using Blog.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Application.Commands.Comment.DeleteComment
{
    public record DeleteCommentCommand(Guid Id) : IRequest<bool>;

    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAppDbContext _context;

        public DeleteCommentCommandHandler(
            ICommentRepository commentRepository,
            ICurrentUserService currentUserService,
            IAppDbContext context)
        {
            _commentRepository = commentRepository;
            _currentUserService = currentUserService;
            _context = context;
        }

        public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(request.Id);
            if (comment == null)
            {
                return false;
            }

            var currentUserId = _currentUserService.UserId;
            var isAdmin = _currentUserService.IsInRole("Admin");

         
            if (comment.UserId != currentUserId && 
                comment.BlogPost.UserId != currentUserId && 
                !isAdmin)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa bình luận này.");
            }

            await _commentRepository.DeleteCommentAsync(request.Id);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
