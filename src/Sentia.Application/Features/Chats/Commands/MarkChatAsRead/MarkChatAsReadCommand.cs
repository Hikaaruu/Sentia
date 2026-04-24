using MediatR;
using Sentia.Application.Common.Interfaces;

namespace Sentia.Application.Features.Chats.Commands.MarkChatAsRead;

public record MarkChatAsReadResult();

public record MarkChatAsReadCommand(
    long ChatId,
    string CurrentUserId,
    string MessageId) : IRequest<MarkChatAsReadResult>, IRequireChatParticipantAuthorization
{
    string IAuthorize.RequestingUserId => CurrentUserId;
}
