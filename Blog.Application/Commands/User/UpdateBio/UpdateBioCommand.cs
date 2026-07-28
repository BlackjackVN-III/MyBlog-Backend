using Blog.Application.DTOs.User;
using Blog.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Application.Commands.User.UpdateBio
{
    public record UpdateBioCommand(UpdateBioRequestDto Dto) : IRequest<UserProfileDto>;

    public class UpdateBioCommandHandler : IRequestHandler<UpdateBioCommand, UserProfileDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdateBioCommandHandler(IUserRepository userRepository, IAppDbContext context, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<UserProfileDto> Handle(UpdateBioCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("Không tìm thấy thông tin tài khoản.");
            }

            user.Bio = request.Dto.Bio;
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
