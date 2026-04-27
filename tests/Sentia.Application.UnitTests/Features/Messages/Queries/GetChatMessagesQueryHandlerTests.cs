using AutoMapper;
using FluentAssertions;
using Sentia.Application.Features.Messages.Queries.GetChatMessages;
using Sentia.Application.Mappings;
using Sentia.Application.UnitTests.Infrastructure;
using Sentia.Domain.Entities;

namespace Sentia.Application.UnitTests.Features.Messages.Queries;

public class GetChatMessagesQueryHandlerTests
{
    private static IMapper CreateMapper() =>
        new MapperConfiguration(cfg => cfg.AddProfile<MessageMappingProfile>()).CreateMapper();

    private static GetChatMessagesQueryHandler CreateSut(TestApplicationDbContext context) =>
        new(context, CreateMapper());

    private static Message MakeMessage(string id, long chatId, DateTime createdAt, string senderId = "user-1") =>
        new() { Id = id, ChatId = chatId, SenderId = senderId, Content = $"Content of {id}", CreatedAt = createdAt };

    [Fact]
    public async Task Handle_NoCursorProvided_ReturnsAllMessagesForChatOrderedChronologically()
    {
        await using var context = TestApplicationDbContext.Create();
        var t = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        context.Messages.AddRange(
            MakeMessage("msg-1", chatId: 1, t),
            MakeMessage("msg-2", chatId: 1, t.AddMinutes(1)),
            MakeMessage("msg-3", chatId: 1, t.AddMinutes(2)));
        await context.SaveChangesAsync(CancellationToken.None);
        var query = new GetChatMessagesQuery(ChatId: 1, CurrentUserId: "user-1", Before: null, Take: 50);

        var result = await CreateSut(context).Handle(query, CancellationToken.None);

        result.Messages.Should().HaveCount(3);
        result.Messages.Select(m => m.CreatedAt).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Handle_WithValidCursor_ReturnsMessagesBeforeCursor()
    {
        await using var context = TestApplicationDbContext.Create();
        var t = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        context.Messages.AddRange(
            MakeMessage("msg-1", chatId: 1, t),
            MakeMessage("msg-2", chatId: 1, t.AddMinutes(1)),
            MakeMessage("msg-3", chatId: 1, t.AddMinutes(2)));
        await context.SaveChangesAsync(CancellationToken.None);
        var query = new GetChatMessagesQuery(ChatId: 1, CurrentUserId: "user-1", Before: "msg-3", Take: 50);

        var result = await CreateSut(context).Handle(query, CancellationToken.None);

        result.Messages.Should().HaveCount(2);
        result.Messages.Should().NotContain(m => m.Id == "msg-3");
    }

    [Fact]
    public async Task Handle_TakeLimitApplied_LimitsReturnedMessages()
    {
        await using var context = TestApplicationDbContext.Create();
        var t = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        for (var i = 0; i < 10; i++)
            context.Messages.Add(MakeMessage($"msg-{i:D2}", chatId: 1, t.AddMinutes(i)));
        await context.SaveChangesAsync(CancellationToken.None);
        var query = new GetChatMessagesQuery(ChatId: 1, CurrentUserId: "user-1", Before: null, Take: 3);

        var result = await CreateSut(context).Handle(query, CancellationToken.None);

        result.Messages.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_MessagesFromDifferentChats_OnlyReturnsChatMessages()
    {
        await using var context = TestApplicationDbContext.Create();
        var t = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        context.Messages.AddRange(
            MakeMessage("msg-chat1", chatId: 1, t),
            MakeMessage("msg-chat2", chatId: 2, t.AddMinutes(1)));
        await context.SaveChangesAsync(CancellationToken.None);
        var query = new GetChatMessagesQuery(ChatId: 1, CurrentUserId: "user-1", Before: null, Take: 50);

        var result = await CreateSut(context).Handle(query, CancellationToken.None);

        result.Messages.Should().HaveCount(1);
        result.Messages[0].Id.Should().Be("msg-chat1");
    }
}
