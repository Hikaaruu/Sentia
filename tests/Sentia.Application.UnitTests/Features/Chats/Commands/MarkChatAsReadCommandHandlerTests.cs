using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Chats.Commands.MarkChatAsRead;
using Sentia.Application.Features.Messages.Events;
using Sentia.Application.UnitTests.Infrastructure;
using Sentia.Domain.Entities;

namespace Sentia.Application.UnitTests.Features.Chats.Commands;

public class MarkChatAsReadCommandHandlerTests
{
    private readonly Mock<IPublisher> _publisher = new();

    private MarkChatAsReadCommandHandler CreateSut(IApplicationDbContext context)
        => new(context, _publisher.Object);

    private static async Task<(TestApplicationDbContext, Message)> SeedChatWithMessageAsync(
        TestApplicationDbContext context,
        string messageId = "msg-1",
        long chatId = 1,
        string senderId = "user-1",
        DateTime? createdAt = null)
    {
        var chat = new Chat { Id = chatId, Type = ChatType.Private, CreatedAt = DateTime.UtcNow };
        context.Chats.Add(chat);
        var message = new Message
        {
            Id = messageId,
            ChatId = chatId,
            SenderId = senderId,
            Content = "Hello",
            CreatedAt = createdAt ?? DateTime.UtcNow
        };
        context.Messages.Add(message);
        await context.SaveChangesAsync(CancellationToken.None);
        return (context, message);
    }

    [Fact]
    public async Task Handle_MessageNotFound_ThrowsNotFoundException()
    {
        await using var context = TestApplicationDbContext.Create();
        var command = new MarkChatAsReadCommand(ChatId: 1, CurrentUserId: "user-2", MessageId: "nonexistent");

        var act = () => CreateSut(context).Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*nonexistent*");
    }

    [Fact]
    public async Task Handle_NoExistingReadStatus_CreatesReadStatusEntry()
    {
        await using var context = TestApplicationDbContext.Create();
        await SeedChatWithMessageAsync(context, messageId: "msg-1", chatId: 1, senderId: "user-1");
        var command = new MarkChatAsReadCommand(ChatId: 1, CurrentUserId: "user-2", MessageId: "msg-1");

        await CreateSut(context).Handle(command, CancellationToken.None);

        var entry = await context.ChatReadStatus
            .FirstOrDefaultAsync(crs => crs.UserId == "user-2" && crs.ChatId == 1);
        entry.Should().NotBeNull();
        entry!.LastReadMessageId.Should().Be("msg-1");
    }

    [Fact]
    public async Task Handle_ExistingReadStatus_UpdatesLastReadMessageId()
    {
        await using var context = TestApplicationDbContext.Create();
        var older = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var newer = new DateTime(2026, 1, 1, 11, 0, 0, DateTimeKind.Utc);

        var chat = new Chat { Id = 1, Type = ChatType.Private, CreatedAt = DateTime.UtcNow };
        context.Chats.Add(chat);

        var oldMsg = new Message { Id = "msg-old", ChatId = 1, SenderId = "user-1", Content = "Old", CreatedAt = older };
        var newMsg = new Message { Id = "msg-new", ChatId = 1, SenderId = "user-1", Content = "New", CreatedAt = newer };
        context.Messages.AddRange(oldMsg, newMsg);

        context.ChatReadStatus.Add(new ChatReadStatus
        {
            UserId = "user-2",
            Chat = chat,
            LastReadMessageId = "msg-old"
        });
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new MarkChatAsReadCommand(ChatId: 1, CurrentUserId: "user-2", MessageId: "msg-new");

        await CreateSut(context).Handle(command, CancellationToken.None);

        var entry = await context.ChatReadStatus
            .FirstOrDefaultAsync(crs => crs.UserId == "user-2" && crs.ChatId == 1);
        entry!.LastReadMessageId.Should().Be("msg-new");
    }

    [Fact]
    public async Task Handle_TargetMessageOlderThanCurrentReadMessage_ReturnsEarlyWithoutSavingOrPublishing()
    {
        // This test uses Mock<IApplicationDbContext> for precise Times.Never verification.
        var older = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        var newer = new DateTime(2026, 1, 1, 11, 0, 0, DateTimeKind.Utc);

        // We need a real InMemory context to seed data, but we need to verify SaveChangesAsync is not called.
        // Strategy: seed via real context, then swap to a wrapping mock that delegates queries to it but spies on SaveChangesAsync.
        await using var seedContext = TestApplicationDbContext.Create();

        var chat = new Chat { Id = 1, Type = ChatType.Private, CreatedAt = DateTime.UtcNow };
        seedContext.Chats.Add(chat);

        var staleMsg = new Message { Id = "msg-stale", ChatId = 1, SenderId = "user-1", Content = "Stale", CreatedAt = older };
        var currentMsg = new Message { Id = "msg-current", ChatId = 1, SenderId = "user-1", Content = "Current", CreatedAt = newer };
        seedContext.Messages.AddRange(staleMsg, currentMsg);

        seedContext.ChatReadStatus.Add(new ChatReadStatus
        {
            UserId = "user-2",
            Chat = chat,
            LastReadMessageId = "msg-current"
        });
        await seedContext.SaveChangesAsync(CancellationToken.None);

        // Use a second InMemory context with same data
        await using var context = TestApplicationDbContext.Create();
        context.Chats.Add(new Chat { Id = 1, Type = ChatType.Private, CreatedAt = DateTime.UtcNow });
        context.Messages.AddRange(
            new Message { Id = "msg-stale", ChatId = 1, SenderId = "user-1", Content = "Stale", CreatedAt = older },
            new Message { Id = "msg-current", ChatId = 1, SenderId = "user-1", Content = "Current", CreatedAt = newer });
        context.ChatReadStatus.Add(new ChatReadStatus
        {
            UserId = "user-2",
            ChatId = 1,
            LastReadMessageId = "msg-current"
        });
        await context.SaveChangesAsync(CancellationToken.None);

        // Now use a Moq mock that wraps the context to spy on SaveChangesAsync
        var mockContext = new Mock<IApplicationDbContext>();
        mockContext.Setup(c => c.Messages).Returns(context.Messages);
        mockContext.Setup(c => c.ChatReadStatus).Returns(context.ChatReadStatus);
        mockContext.Setup(c => c.Chats).Returns(context.Chats);
        mockContext.Setup(c => c.ChatParticipants).Returns(context.ChatParticipants);
        mockContext.Setup(c => c.Users).Returns(context.Users);

        var command = new MarkChatAsReadCommand(ChatId: 1, CurrentUserId: "user-2", MessageId: "msg-stale");

        await CreateSut(mockContext.Object).Handle(command, CancellationToken.None);

        mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _publisher.Verify(p => p.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidUpdate_PublishesMessageReadEventWithCorrectFields()
    {
        await using var context = TestApplicationDbContext.Create();
        await SeedChatWithMessageAsync(context, messageId: "msg-1", chatId: 1, senderId: "user-1");
        var command = new MarkChatAsReadCommand(ChatId: 1, CurrentUserId: "user-2", MessageId: "msg-1");

        await CreateSut(context).Handle(command, CancellationToken.None);

        _publisher.Verify(p => p.Publish(
            It.Is<MessageReadEvent>(e =>
                e.MessageId == "msg-1" &&
                e.ChatId == 1 &&
                e.SenderId == "user-1" &&
                e.ReadByUserId == "user-2"),
            CancellationToken.None),
            Times.Once);
    }
}
