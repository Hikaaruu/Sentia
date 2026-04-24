using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;

namespace Sentia.Application.Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>(IApplicationDbContext context)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IRequireChatParticipantAuthorization authRequest)
        {
            var isParticipant = await context.ChatParticipants
                .AnyAsync(
                    cp => cp.ChatId == authRequest.ChatId && cp.UserId == authRequest.RequestingUserId,
                    cancellationToken);

            if (!isParticipant)
                throw new ForbiddenException("You are not a participant of this chat.");
        }

        return await next();
    }
}
