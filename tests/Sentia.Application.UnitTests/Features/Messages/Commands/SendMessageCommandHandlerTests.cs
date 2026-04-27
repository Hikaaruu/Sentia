using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Features.Messages.Commands.SendMessage;
using Sentia.Application.Features.Messages.Events;
using Sentia.Application.UnitTests.Infrastructure;
using Sentia.Domain.Entities;

namespace Sentia.Application.UnitTests.Features.Messages.Commands;

public class SendMessageCommandHandlerTests
{
    private readonly Mock<IPublisher> _publisher = new();

    private SendMessageCommandHandler CreateSut(TestApplicationDbContext context)
        => new(context, _publisher.Object);

    private static async Task<TestApplicationDbContext> SeedChatWithParticipantsAsync(
        TestApplicationDbContext context,
        long chatId,
        params string[] participantIds)
    {
        var chat = new Chat { Id = chatId, Type = ChatType.Private, CreatedAt = DateTime.UtcNow };
        context.Chats.Add(chat);
        foreach (var userId in participantIds)
            context.ChatParticipants.Add(new ChatParticipant { Chat = chat, UserId = userId, JoinedAt = DateTime.UtcNow });
        await context.SaveChangesAsync(CancellationToken.None);
        return context;
    }

    [Fact]
    public async Task Handle_DuplicateMessageId_ThrowsValidationException()
    {
        await using var context = TestApplicationDbContext.Create();
        await SeedChatWithParticipantsAsync(context, chatId: 1, "user-1", "user-2");
        context.Messages.Add(new Message { Id = "msg-dup", ChatId = 1, SenderId = "user-1", Content = "hi", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new SendMessageCommand("msg-dup", ChatId: 1, SenderId: "user-1", Content: "Hello again");

        var act = () => CreateSut(context).Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ValidMessage_PersistsMessageToDatabase()
    {
        await using var context = TestApplicationDbContext.Create();
        await SeedChatWithParticipantsAsync(context, chatId: 1, "user-1", "user-2");

        var command = new SendMessageCommand("msg-new", ChatId: 1, SenderId: "user-1", Content: "Hello world");

        await CreateSut(context).Handle(command, CancellationToken.None);

        var saved = await context.Messages.FindAsync(["msg-new"]);
        saved.Should().NotBeNull();
        saved!.Content.Should().Be("Hello world");
        saved.SenderId.Should().Be("user-1");
    }

    [Fact]
    public async Task Handle_ValidMessage_UpdatesChatLastMessageAt()
    {
        await using var context = TestApplicationDbContext.Create();
        await SeedChatWithParticipantsAsync(context, chatId: 1, "user-1", "user-2");

        var command = new SendMessageCommand("msg-new", ChatId: 1, SenderId: "user-1", Content: "Hello");

        await CreateSut(context).Handle(command, CancellationToken.None);

        var chat = await context.Chats.FindAsync([1L]);
        chat!.LastMessageAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ValidMessage_PublishesMessageCreatedEventWithAllParticipantIds()
    {
        await using var context = TestApplicationDbContext.Create();
        await SeedChatWithParticipantsAsync(context, chatId: 1, "user-1", "user-2");

        var command = new SendMessageCommand("msg-1", ChatId: 1, SenderId: "user-1", Content: "Hello");

        await CreateSut(context).Handle(command, CancellationToken.None);

        _publisher.Verify(p => p.Publish(
            It.Is<MessageCreatedEvent>(e =>
                e.MessageId == "msg-1" &&
                e.ChatId == 1 &&
                e.SenderId == "user-1" &&
                e.Content == "Hello" &&
                e.ParticipantUserIds.Contains("user-1") &&
                e.ParticipantUserIds.Contains("user-2")),
            CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidMessage_ReturnsCorrectMessageId()
    {
        await using var context = TestApplicationDbContext.Create();
        await SeedChatWithParticipantsAsync(context, chatId: 1, "user-1", "user-2");

        var command = new SendMessageCommand("msg-xyz", ChatId: 1, SenderId: "user-1", Content: "Hi");

        var result = await CreateSut(context).Handle(command, CancellationToken.None);

        result.MessageId.Should().Be("msg-xyz");
    }
}
