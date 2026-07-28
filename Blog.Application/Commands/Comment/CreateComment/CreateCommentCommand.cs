using Blog.Application.DTOs.Comment;
using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Application.Commands.Comment.CreateComment
{
    public record CreateCommentCommand(CreateCommentRequestDto Dto) : IRequest<CommentDto>;

    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CommentDto>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAppDbContext _context;
        private readonly INotificationService _notificationService;
        public CreateCommentCommandHandler(
            ICommentRepository commentRepository,
            IPostRepository postRepository,
            ICurrentUserService currentUserService,
            IAppDbContext context,
            INotificationService notificationService)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _currentUserService = currentUserService;
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<CommentDto> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
         
            var blog = await _postRepository.GetBlogByIdAsync(request.Dto.PostId);
            if (blog == null)
            {
                throw new Exception("Bài viết không tồn tại.");
            }

        
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

        
            var comment = request.Dto.toCommentFromCreateDto();
            comment.Id = Guid.NewGuid();
            comment.UserId = userId;

            await _commentRepository.CreateCommentAsync(comment);
            await _context.SaveChangesAsync(cancellationToken);

            // Fetch lại để load kèm thông tin Author khi trả về DTO
            var createdComment = await _commentRepository.GetCommentByIdAsync(comment.Id);
            var commentDto = createdComment!.toCommentDto();

            var username = commentDto.User?.Username ?? "Ai đó";
            var content = commentDto.Content;
            // Gửi Live Comment (chỉ truyền username và content)
            await _notificationService.SendNewCommentEventAsync(blog.Id, username, content);
            // Gửi thông báo đẩy cho tác giả (nếu người bình luận khác tác giả bài viết)
            if (blog.UserId != comment.UserId)
            {
                var message = $"{username} đã bình luận vào bài viết '{blog.Title}': {content}";
                await _notificationService.SendCommentNotificationAsync(blog.Id, message, blog.UserId);
            }
            return commentDto;

        }
    }
}
