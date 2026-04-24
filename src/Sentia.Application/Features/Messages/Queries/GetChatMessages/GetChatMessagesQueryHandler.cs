using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Dtos;

namespace Sentia.Application.Features.Messages.Queries.GetChatMessages;

public class GetChatMessagesQueryHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<GetChatMessagesQuery, List<MessageDto>>
{
    public async Task<List<MessageDto>> Handle(
        GetChatMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var isParticipant = await context.ChatParticipants
            .AnyAsync(cp => cp.ChatId == request.ChatId && cp.UserId == request.CurrentUserId,
                cancellationToken);

        if (!isParticipant)
            throw new ValidationException("ChatId", "You are not a participant of this chat.");

        var take = Math.Clamp(request.Take, 1, 100);

        var query = context.Messages
            .Where(m => m.ChatId == request.ChatId);

        if (request.Before.HasValue)
            query = query.Where(m => m.Id < request.Before.Value);

        var messages = await query
            .OrderByDescending(m => m.Id)
            .Take(take)
            .OrderBy(m => m.Id)
            .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return messages;
    }
}
