using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sentia.Application.Common.Interfaces;
using Sentia.Infrastructure.RealTime.Services;

namespace Sentia.Infrastructure.RealTime.Hubs;

[Authorize]
public class ChatHub(IApplicationDbContext context, PresenceTracker presenceTracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        var isFirstConnection = presenceTracker.UserConnected(userId, Context.ConnectionId);
        if (isFirstConnection)
        {
            await Clients.Others.SendAsync("UserIsOnline", userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier!;
        var isOffline = presenceTracker.UserDisconnected(userId, Context.ConnectionId);
        if (isOffline)
        {
            await Clients.Others.SendAsync("UserIsOffline", userId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public Task<IEnumerable<string>> GetOnlineUsers()
        => Task.FromResult(presenceTracker.GetOnlineUsers());

    public async Task SendTyping(long chatId)
    {
        var senderId = Context.UserIdentifier!;

        var isParticipant = await context.ChatParticipants
            .AnyAsync(cp => cp.ChatId == chatId && cp.UserId == senderId);

        if (!isParticipant)
            return;

        var recipientIds = await context.ChatParticipants
            .Where(cp => cp.ChatId == chatId && cp.UserId != senderId)
            .Select(cp => cp.UserId)
            .ToListAsync();

        await Clients.Users(recipientIds)
            .SendAsync("ReceiveTyping", new { chatId, senderId });
    }
}
