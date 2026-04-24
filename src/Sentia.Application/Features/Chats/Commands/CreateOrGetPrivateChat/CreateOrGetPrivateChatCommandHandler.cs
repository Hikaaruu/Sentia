using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;
using Sentia.Domain.Entities;

namespace Sentia.Application.Features.Chats.Commands.CreateOrGetPrivateChat;

public class CreateOrGetPrivateChatCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<CreateOrGetPrivateChatCommand, CreateOrGetPrivateChatResult>
{
    public async Task<CreateOrGetPrivateChatResult> Handle(
        CreateOrGetPrivateChatCommand request,
        CancellationToken cancellationToken)
    {
        var recipient = await context.Users
            .Where(u => u.Id == request.RecipientUserId)
            .Select(u => new { u.Id, u.UserName })
            .FirstOrDefaultAsync(cancellationToken);

        if (recipient is null)
            throw new NotFoundException("User", request.RecipientUserId);

        // Find existing private chat between these two users
        var existingChat = await context.Chats
            .Where(c => c.Type == ChatType.Private
                    && c.Participants.Any(p => p.UserId == request.CurrentUserId)
                    && c.Participants.Any(p => p.UserId == request.RecipientUserId))
            .Select(c => new { c.Id, c.CreatedAt })
            .FirstOrDefaultAsync(cancellationToken);

        if (existingChat is not null)
            return new CreateOrGetPrivateChatResult(
                existingChat.Id,
                IsNew: false,
                recipient.Id,
                recipient.UserName,
                existingChat.CreatedAt);

        var now = DateTime.UtcNow;

        var chat = new Chat
        {
            Type = ChatType.Private,
            Title = null,
            CreatedAt = now
        };

        context.Chats.Add(chat);

        context.ChatParticipants.Add(new ChatParticipant { Chat = chat, UserId = request.CurrentUserId, JoinedAt = now });
        context.ChatParticipants.Add(new ChatParticipant { Chat = chat, UserId = request.RecipientUserId, JoinedAt = now });

        context.ChatReadStatus.Add(new ChatReadStatus { Chat = chat, UserId = request.CurrentUserId, LastReadMessageId = null });
        context.ChatReadStatus.Add(new ChatReadStatus { Chat = chat, UserId = request.RecipientUserId, LastReadMessageId = null });

        await context.SaveChangesAsync(cancellationToken);

        return new CreateOrGetPrivateChatResult(
            chat.Id,
            IsNew: true,
            recipient.Id,
            recipient.UserName,
            now);
    }
}
