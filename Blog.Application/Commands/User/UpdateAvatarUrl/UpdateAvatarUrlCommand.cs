using Blog.Application.DTOs.User;
using Blog.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Application.Commands.User.UpdateAvatarUrl
{
    public record UpdateAvatarUrlCommand(string AvatarUrl) : IRequest<UserProfileDto>;

    public class UpdateAvatarUrlCommandHandler : IRequestHandler<UpdateAvatarUrlCommand, UserProfileDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdateAvatarUrlCommandHandler(IUserRepository userRepository, IAppDbContext context, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<UserProfileDto> Handle(UpdateAvatarUrlCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("Không tìm thấy thông tin tài khoản.");
            }

            user.AvatarUrl = request.AvatarUrl;
            await _context.SaveChangesAsync(cancellationToken);

            return new UserProfileDto
            {
                Username = user.Username,
                Email = user.Email,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl
            };
        }
    }
}
