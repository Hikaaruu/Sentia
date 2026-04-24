using MediatR;
using Sentia.Application.Features.Messages.Dtos;

namespace Sentia.Application.Features.Messages.Queries.GetChatMessages;

public record GetChatMessagesQuery(
    long ChatId,
    string CurrentUserId,
    long? Before,
    int Take = 50) : IRequest<List<MessageDto>>;
