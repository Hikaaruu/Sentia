using MediatR;
using Sentia.Application.Features.Chats.Dtos;

namespace Sentia.Application.Features.Chats.Queries.GetUserChats;

public record GetUserChatsResult(List<ChatSummaryDto> Chats);
public record GetUserChatsQuery(string UserId) : IRequest<GetUserChatsResult>;
