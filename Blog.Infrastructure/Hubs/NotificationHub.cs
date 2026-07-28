using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Infrastructure.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinGroupAsync(string postId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Post_{postId}");
        }
        public async Task LeavePostGroup(string postId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Post_{postId}");
        }
    }
}
