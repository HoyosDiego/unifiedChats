using Microsoft.AspNetCore.SignalR;
using UnifiedChat.Domain.Models;

namespace UnifiedChat.Api.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(StreamMessage message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}

