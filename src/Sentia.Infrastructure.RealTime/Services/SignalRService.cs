using Microsoft.AspNetCore.SignalR;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Dtos;
using Sentia.Infrastructure.RealTime.Hubs;

namespace Sentia.Infrastructure.RealTime.Services;

public class SignalRService(IHubContext<ChatHub> hubContext) : ISignalRService
{
    public async Task BroadcastNewMessageAsync(
        IEnumerable<string> userIds,
        NewMessagePayload payload,
        CancellationToken cancellationToken = default)
    {
        await hubContext.Clients
            .Users([.. userIds])
            .SendAsync("ReceiveNewMessage", payload, cancellationToken);
    }

    public async Task BroadcastSentimentUpdateAsync(
        IEnumerable<string> userIds,
        SentimentUpdatePayload payload,
        CancellationToken cancellationToken = default)
    {
        await hubContext.Clients
            .Users([.. userIds])
            .SendAsync("UpdateMessageSentiment", payload, cancellationToken);
    }

    public async Task BroadcastReadReceiptAsync(
        string recipientUserId,
        ReadReceiptPayload payload,
        CancellationToken cancellationToken = default)
    {
        await hubContext.Clients
            .User(recipientUserId)
            .SendAsync("ReceiveReadReceipt", payload, cancellationToken);
    }

    public async Task BroadcastTypingAsync(
        IEnumerable<string> recipientUserIds,
        long chatId,
        string senderId,
        CancellationToken cancellationToken = default)
    {
        await hubContext.Clients
            .Users([.. recipientUserIds])
            .SendAsync("ReceiveTyping", new { chatId, senderId }, cancellationToken);
    }
}
