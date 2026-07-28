using Blog.Application.Commands.Comment.CreateComment;
using Blog.Application.DTOs.Comment;
using Blog.Application.DTOs.User;
using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Blog.Application.Tests.Commands.Comment.CreateComment
{
    public class CreateCommentCommandTests
    {
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IAppDbContext> _contextMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly CreateCommentCommandHandler _handler;

        public CreateCommentCommandTests()
        {
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _postRepositoryMock = new Mock<IPostRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _contextMock = new Mock<IAppDbContext>();
            _notificationServiceMock = new Mock<INotificationService>();

            _handler = new CreateCommentCommandHandler(
                _commentRepositoryMock.Object,
                _postRepositoryMock.Object,
                _currentUserServiceMock.Object,
                _contextMock.Object,
                _notificationServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenValidRequestAndCommenterIsNotAuthor_ShouldCreateCommentAndNotifyAuthor()
        {
            // 1. Arrange
            var authorId = Guid.NewGuid();
            var commenterId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            var blog = new BlogPost { Id = postId, Title = "Clean Architecture", UserId = authorId };
            var requestDto = new CreateCommentRequestDto { PostId = postId, Content = "Bình luận hay quá!" };
            var command = new CreateCommentCommand(requestDto);

            // Mock trả về thông tin bài viết để lấy ID tác giả
            _postRepositoryMock.Setup(x => x.GetBlogByIdAsync(postId)).ReturnsAsync(blog);
            
            // Giả lập người bình luận đang đăng nhập
            _currentUserServiceMock.Setup(x => x.UserId).Returns(commenterId);

            // Mock việc lưu trữ và nạp lại bình luận kèm thông tin người dùng
            var createdComment = new Blog.Domain.Entities.Comment
            {
                Id = Guid.NewGuid(),
                BlogPostId = postId,
                Content = requestDto.Content,
                UserId = commenterId,
                Author = new User { Id = commenterId, Username = "commenter_user" }
            };
            _commentRepositoryMock.Setup(x => x.GetCommentByIdAsync(It.IsAny<Guid>())).ReturnsAsync(createdComment);

            // 2. Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // 3. Assert - Kiểm chứng kết quả trả về
            result.Should().NotBeNull();
            result.Content.Should().Be(requestDto.Content);
            result.User?.Username.Should().Be("commenter_user");

            // 3. Assert - Kiểm chứng các phương thức được gọi
            _commentRepositoryMock.Verify(x => x.CreateCommentAsync(It.Is<Blog.Domain.Entities.Comment>(
                c => c.BlogPostId == postId && c.Content == requestDto.Content && c.UserId == commenterId
            )), Times.Once);

            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Kiểm chứng việc phát tin Live-comment
            _notificationServiceMock.Verify(x => x.SendNewCommentEventAsync(
                postId, "commenter_user", requestDto.Content
            ), Times.Once);

            // Kiểm chứng việc đẩy thông báo tới tác giả bài viết
            _notificationServiceMock.Verify(x => x.SendCommentNotificationAsync(
                postId, It.IsAny<string>(), authorId
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCommenterIsAuthor_ShouldCreateCommentButNotNotifyAuthor()
        {
            // 1. Arrange
            var authorId = Guid.NewGuid(); // Tác giả bài viết tự bình luận
            var postId = Guid.NewGuid();

            var blog = new BlogPost { Id = postId, Title = "Clean Architecture", UserId = authorId };
            var requestDto = new CreateCommentRequestDto { PostId = postId, Content = "Tôi tự bình luận bài của tôi" };
            var command = new CreateCommentCommand(requestDto);

            _postRepositoryMock.Setup(x => x.GetBlogByIdAsync(postId)).ReturnsAsync(blog);
            _currentUserServiceMock.Setup(x => x.UserId).Returns(authorId);

            var createdComment = new Blog.Domain.Entities.Comment
            {
                Id = Guid.NewGuid(),
                BlogPostId = postId,
                Content = requestDto.Content,
                UserId = authorId,
                Author = new User { Id = authorId, Username = "author_user" }
            };
            _commentRepositoryMock.Setup(x => x.GetCommentByIdAsync(It.IsAny<Guid>())).ReturnsAsync(createdComment);

            // 2. Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // 3. Assert
            result.Should().NotBeNull();
            
            // Đảm bảo Live-comment vẫn được phát
            _notificationServiceMock.Verify(x => x.SendNewCommentEventAsync(
                postId, "author_user", requestDto.Content
            ), Times.Once);

            // Đảm bảo KHÔNG gửi thông báo cá nhân cho chính mình
            _notificationServiceMock.Verify(x => x.SendCommentNotificationAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()
            ), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenBlogDoesNotExist_ShouldThrowException()
        {
            // 1. Arrange
            var postId = Guid.NewGuid();
            var requestDto = new CreateCommentRequestDto { PostId = postId, Content = "Bình luận" };
            var command = new CreateCommentCommand(requestDto);

            // Giả lập bài viết không tồn tại
            _postRepositoryMock.Setup(x => x.GetBlogByIdAsync(postId)).ReturnsAsync((BlogPost?)null);

            // 2. Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // 3. Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Bài viết không tồn tại.");
        }

        [Fact]
        public async Task Handle_WhenUserNotLoggedIn_ShouldThrowUnauthorizedAccessException()
        {
            // 1. Arrange
            var postId = Guid.NewGuid();
            var blog = new BlogPost { Id = postId, Title = "Clean Architecture" };
            var requestDto = new CreateCommentRequestDto { PostId = postId, Content = "Bình luận" };
            var command = new CreateCommentCommand(requestDto);

            _postRepositoryMock.Setup(x => x.GetBlogByIdAsync(postId)).ReturnsAsync(blog);
            
            // Người dùng chưa đăng nhập
            _currentUserServiceMock.Setup(x => x.UserId).Returns((Guid?)null);

            // 2. Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // 3. Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Người dùng chưa đăng nhập.");
        }
    }
}
