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

        public CreateCommentCommandHandler(
            ICommentRepository commentRepository,
            IPostRepository postRepository,
            ICurrentUserService currentUserService,
            IAppDbContext context)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _currentUserService = currentUserService;
            _context = context;
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
            return createdComment!.toCommentDto();
            
        }
    }
}
