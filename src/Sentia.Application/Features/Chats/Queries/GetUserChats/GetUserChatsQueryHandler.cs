using MediatR;
using Sentia.Application.Common.Interfaces;


namespace Sentia.Application.Features.Chats.Queries.GetUserChats;

public class GetUserChatsQueryHandler(IChatQueryService chatQueryService)
    : IRequestHandler<GetUserChatsQuery, GetUserChatsResult>
{
    public async Task<GetUserChatsResult> Handle(GetUserChatsQuery request, CancellationToken cancellationToken)
    {
        var chats = await chatQueryService.GetUserChatsAsync(request.UserId, cancellationToken);
        return new GetUserChatsResult(chats);
    }
}
