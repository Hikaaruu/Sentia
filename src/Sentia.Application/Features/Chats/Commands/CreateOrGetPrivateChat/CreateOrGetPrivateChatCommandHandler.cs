using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;
using Sentia.Domain.Entities;

namespace Sentia.Application.Features.Chats.Commands.CreateOrGetPrivateChat;

public class CreateOrGetPrivateChatCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService)
    : IRequestHandler<CreateOrGetPrivateChatCommand, CreateOrGetPrivateChatResult>
{
    public async Task<CreateOrGetPrivateChatResult> Handle(
        CreateOrGetPrivateChatCommand request,
        CancellationToken cancellationToken)
    {
        var recipientExists = await identityService.UserExistsAsync(request.RecipientUserId);
        if (!recipientExists)
            throw new ValidationException("RecipientUserId", "User not found.");

        // Find existing private chat between these two users
        var existingChatId = await context.Chats
            .Where(c => c.Type == ChatType.Private
                    && c.Participants.Any(p => p.UserId == request.CurrentUserId)
                    && c.Participants.Any(p => p.UserId == request.RecipientUserId))
            .Select(c => c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingChatId != default)
            return new CreateOrGetPrivateChatResult(existingChatId, IsNew: false);


        var now = DateTime.UtcNow;

        var chat2 = new Chat
        {
            Type = ChatType.Private,
            Title = null,
            CreatedAt = now
        };

        context.Chats.Add(chat2);

        context.ChatParticipants.Add(new ChatParticipant { Chat = chat2, UserId = request.CurrentUserId, JoinedAt = now });
        context.ChatParticipants.Add(new ChatParticipant { Chat = chat2, UserId = request.RecipientUserId, JoinedAt = now });

        context.ChatReadStatus.Add(new ChatReadStatus { Chat = chat2, UserId = request.CurrentUserId, LastReadMessageId = null });
        context.ChatReadStatus.Add(new ChatReadStatus { Chat = chat2, UserId = request.RecipientUserId, LastReadMessageId = null });

        await context.SaveChangesAsync(cancellationToken);

        return new CreateOrGetPrivateChatResult(chat2.Id, IsNew: true);
    }
}
