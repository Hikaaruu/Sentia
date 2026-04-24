using MediatR;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Dtos;

namespace Sentia.Application.Features.Messages.Queries.GetChatMessages;

public record GetChatMessagesResult(List<MessageDto> Messages);

public record GetChatMessagesQuery(
    long ChatId,
    string CurrentUserId,
    string? Before,
    int Take = 50) : IRequest<GetChatMessagesResult>, IRequireChatParticipantAuthorization
{
    string IAuthorize.RequestingUserId => CurrentUserId;
}
