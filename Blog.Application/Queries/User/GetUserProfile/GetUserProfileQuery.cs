using Blog.Application.DTOs.User;
using Blog.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Application.Queries.User.GetUserProfile
{
    public record GetUserProfileQuery : IRequest<UserProfileDto>;

    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetUserProfileQueryHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("Không tìm thấy thông tin tài khoản.");
            }

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
