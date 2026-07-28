using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Interfaces
{
    public interface INotificationService
    {
       
        Task SendNewCommentEventAsync(Guid postId, string username, string content);

        Task SendCommentNotificationAsync(Guid postId, string message, Guid authorId);
    }
}
