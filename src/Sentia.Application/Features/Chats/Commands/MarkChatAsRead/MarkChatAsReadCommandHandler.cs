using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Events;
using Sentia.Domain.Entities;

namespace Sentia.Application.Features.Chats.Commands.MarkChatAsRead;

public class MarkChatAsReadCommandHandler(
    IApplicationDbContext context,
    IPublisher publisher)
    : IRequestHandler<MarkChatAsReadCommand, MarkChatAsReadResult>
{
    public async Task<MarkChatAsReadResult> Handle(
        MarkChatAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var message = await context.Messages
            .Where(m => m.Id == request.MessageId && m.ChatId == request.ChatId)
            .Select(m => new { m.Id, m.SenderId })
            .FirstOrDefaultAsync(cancellationToken);

        if (message is null)
            throw new NotFoundException("Message", request.MessageId);

        var readStatus = await context.ChatReadStatus
            .FirstOrDefaultAsync(
                crs => crs.UserId == request.CurrentUserId && crs.ChatId == request.ChatId,
                cancellationToken);

        if (readStatus is null)
        {
            context.ChatReadStatus.Add(new ChatReadStatus
            {
                UserId = request.CurrentUserId,
                ChatId = request.ChatId,
                LastReadMessageId = request.MessageId,
                ReadAt = DateTime.UtcNow
            });
        }
        else
        {
            readStatus.LastReadMessageId = request.MessageId;
            readStatus.ReadAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new MessageReadEvent(
            MessageId: request.MessageId,
            ChatId: request.ChatId,
            SenderId: message.SenderId,
            ReadByUserId: request.CurrentUserId),
            cancellationToken);

        return new MarkChatAsReadResult();
    }
}
