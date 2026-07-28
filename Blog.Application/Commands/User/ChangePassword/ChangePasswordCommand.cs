using Blog.Application.DTOs.User;
using Blog.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Application.Commands.User.ChangePassword
{
    public record ChangePasswordCommand(ChangePasswordRequestDto Dto) : IRequest<bool>;

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;

        public ChangePasswordCommandHandler(IAuthService authService, ICurrentUserService currentUserService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

            var success = await _authService.ChangePasswordAsync(
                userId, 
                request.Dto.CurrentPassword, 
                request.Dto.NewPassword
            );

            if (!success)
            {
                throw new Exception("Mật khẩu hiện tại không chính xác hoặc mật khẩu mới không hợp lệ.");
            }

            return true;
        }
    }
}
