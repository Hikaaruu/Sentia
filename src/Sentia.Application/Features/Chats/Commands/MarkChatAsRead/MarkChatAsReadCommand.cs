using MediatR;
using Sentia.Application.Common.Interfaces;

namespace Sentia.Application.Features.Chats.Commands.MarkChatAsRead;


public record MarkChatAsReadCommand(
    long ChatId,
    string CurrentUserId,
    string MessageId) : IRequest, IRequireChatParticipantAuthorization
{
    string IAuthorize.RequestingUserId => CurrentUserId;
}
