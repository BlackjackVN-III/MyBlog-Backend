using Blog.Application.Interfaces;
using Blog.Application.Queries.User.GetUserProfile;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Tests.Queries.User.GetUserProfile
{
    public class GetUserProfileQueryTests
    {
        
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly GetUserProfileQueryHandler _handler;
        public GetUserProfileQueryTests()
        {
           
            _userRepositoryMock = new Mock<IUserRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
          
            _handler = new GetUserProfileQueryHandler(
                _userRepositoryMock.Object,
                _currentUserServiceMock.Object
            );
        }
        [Fact] 
        public async Task Handle_WhenUserIsLoggedInAndExists_ShouldReturnUserProfile()
        {
            var userId = Guid.NewGuid();
            var existingUser = new Blog.Domain.Entities.User
            {
                Id = userId,
                Username = "blackjackvn",
                Email = "bjvn@test.com",
                Bio = "Hello world",
                AvatarUrl = "http://avatar-link.jpg"
            };
          
            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

          
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(existingUser);
            var query = new GetUserProfileQuery();
          
            var result = await _handler.Handle(query, CancellationToken.None);
     
            result.Should().NotBeNull();
            result.Username.Should().Be(existingUser.Username);
            result.Email.Should().Be(existingUser.Email);
            result.Bio.Should().Be(existingUser.Bio);
            result.AvatarUrl.Should().Be(existingUser.AvatarUrl);
        }
        [Fact]
        public async Task Handle_WhenUserNotLoggedIn_ShouldThrowUnauthorizedAccessException()
        {
 
            _currentUserServiceMock.Setup(x => x.UserId).Returns((Guid?)null);

            var query = new GetUserProfileQuery();
         
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
          
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Người dùng chưa đăng nhập.");
        }
        [Fact]
        public async Task Handle_WhenUserNotFoundInDatabase_ShouldThrowException()
        {
          
            var userId = Guid.NewGuid();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

   
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((Blog.Domain.Entities.User?)null);
            var query = new GetUserProfileQuery();
          
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
          
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Không tìm thấy thông tin tài khoản.");
        }
    }
}
