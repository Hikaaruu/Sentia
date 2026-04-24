using MediatR;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Dtos;
using Sentia.Application.Features.Messages.Events;

namespace Sentia.Application.Features.Messages.EventHandlers;

public class BroadcastReadReceiptEventHandler(ISignalRService signalRService)
    : INotificationHandler<MessageReadEvent>
{
    public async Task Handle(MessageReadEvent notification, CancellationToken cancellationToken)
    {
        var payload = new ReadReceiptPayload(
            MessageId: notification.MessageId,
            ChatId: notification.ChatId,
            ReadByUserId: notification.ReadByUserId);

        await signalRService.BroadcastReadReceiptAsync(
            notification.SenderId,
            payload,
            cancellationToken);
    }
}
