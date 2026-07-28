using Blog.Application.Interfaces;
using Blog.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendNewCommentEventAsync(Guid postId, string username, string content)
        {
           
            await _hubContext.Clients.Group($"Post_{postId}")
                .SendAsync("ReceiveNewComment", username, content);
        }
        public async Task SendCommentNotificationAsync(Guid postId, string message, Guid authorId)
        {
            // Gửi sự kiện ReceiveNotification kèm theo postId để điều hướng và chuỗi message thô
            await _hubContext.Clients.User(authorId.ToString())
                .SendAsync("ReceiveNotification", postId, message);
        }
    }
}
