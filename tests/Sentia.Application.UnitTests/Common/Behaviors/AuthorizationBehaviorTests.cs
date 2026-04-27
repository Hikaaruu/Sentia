using FluentAssertions;
using MediatR;
using Moq;
using Sentia.Application.Common.Behaviors;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.UnitTests.Infrastructure;
using Sentia.Domain.Entities;

namespace Sentia.Application.UnitTests.Common.Behaviors;

public class AuthorizationBehaviorTests
{
    private record PlainRequest : IRequest<string>;

    private record AuthorizedRequest(long ChatId, string RequestingUserId)
        : IRequest<string>, IRequireChatParticipantAuthorization;

    private static RequestHandlerDelegate<string> NextReturning(string value)
        => () => Task.FromResult(value);

    [Fact]
    public async Task Handle_RequestDoesNotRequireAuthorization_CallsNext()
    {
        await using var context = TestApplicationDbContext.Create();
        var sut = new AuthorizationBehavior<PlainRequest, string>(context);
        var nextCalled = false;
        RequestHandlerDelegate<string> next = () => { nextCalled = true; return Task.FromResult("ok"); };

        var result = await sut.Handle(new PlainRequest(), next, CancellationToken.None);

        nextCalled.Should().BeTrue();
        result.Should().Be("ok");
    }

    [Fact]
    public async Task Handle_UserIsParticipant_CallsNext()
    {
        await using var context = TestApplicationDbContext.Create();
        context.ChatParticipants.Add(new ChatParticipant
        {
            ChatId = 1,
            UserId = "user-1",
            JoinedAt = DateTime.UtcNow,
            Chat = new Chat { Id = 1, Type = ChatType.Private, CreatedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync(CancellationToken.None);
        var sut = new AuthorizationBehavior<AuthorizedRequest, string>(context);
        var request = new AuthorizedRequest(ChatId: 1, RequestingUserId: "user-1");

        var result = await sut.Handle(request, NextReturning("ok"), CancellationToken.None);

        result.Should().Be("ok");
    }

    [Fact]
    public async Task Handle_UserIsNotParticipant_ThrowsForbiddenExceptionWithoutCallingNext()
    {
        await using var context = TestApplicationDbContext.Create();
        var sut = new AuthorizationBehavior<AuthorizedRequest, string>(context);
        var request = new AuthorizedRequest(ChatId: 1, RequestingUserId: "user-99");
        var nextCalled = false;
        RequestHandlerDelegate<string> next = () => { nextCalled = true; return Task.FromResult("ok"); };

        var act = () => sut.Handle(request, next, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
        nextCalled.Should().BeFalse();
    }
}
