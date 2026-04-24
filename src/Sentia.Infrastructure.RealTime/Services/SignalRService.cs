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
}
