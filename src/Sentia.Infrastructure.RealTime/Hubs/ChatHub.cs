using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sentia.Application.Common.Interfaces;

namespace Sentia.Infrastructure.RealTime.Hubs;

[Authorize]
public class ChatHub(IApplicationDbContext context) : Hub
{
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
