using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Dtos;

namespace Sentia.Application.Features.Messages.Queries.GetChatMessages;

public class GetChatMessagesQueryHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<GetChatMessagesQuery, GetChatMessagesResult>
{
    public async Task<GetChatMessagesResult> Handle(
        GetChatMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Messages
            .Where(m => m.ChatId == request.ChatId);

        if (request.Before is not null)
        {
            var cursorMessage = await context.Messages
                .Where(m => m.Id == request.Before)
                .Select(m => new { m.CreatedAt, m.Id })
                .FirstOrDefaultAsync(cancellationToken);

            if (cursorMessage is not null)
            {
                query = query.Where(m => m.CreatedAt < cursorMessage.CreatedAt ||
                                        (m.CreatedAt == cursorMessage.CreatedAt && string.Compare(m.Id, cursorMessage.Id) < 0));
            }
        }

        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Take(request.Take)
            .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        messages.Reverse();

        return new GetChatMessagesResult(messages);
    }
}
