using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Features.Chats.Commands.CreateOrGetPrivateChat;
using Sentia.Application.UnitTests.Infrastructure;
using Sentia.Domain.Entities;

namespace Sentia.Application.UnitTests.Features.Chats.Commands;

public class CreateOrGetPrivateChatCommandHandlerTests
{
    private static CreateOrGetPrivateChatCommandHandler CreateSut(TestApplicationDbContext context)
        => new(context);

    private static async Task<(TestApplicationDbContext, Chat)> SeedUserAndChatAsync(
        TestApplicationDbContext context,
        string currentUserId,
        string recipientUserId,
        string? recipientLastReadMessageId = null)
    {
        context.Users.Add(new User { Id = recipientUserId, UserName = recipientUserId + "_name" });

        var chat = new Chat { Id = 1, Type = ChatType.Private, CreatedAt = DateTime.UtcNow };
        context.Chats.Add(chat);
        context.ChatParticipants.Add(new ChatParticipant { Chat = chat, UserId = currentUserId, JoinedAt = DateTime.UtcNow });
        context.ChatParticipants.Add(new ChatParticipant { Chat = chat, UserId = recipientUserId, JoinedAt = DateTime.UtcNow });

        if (recipientLastReadMessageId is not null)
        {
            var msg = new Message { Id = recipientLastReadMessageId, ChatId = 1, SenderId = currentUserId, Content = "hi", CreatedAt = DateTime.UtcNow };
            context.Messages.Add(msg);
            context.ChatReadStatus.Add(new ChatReadStatus
            {
                UserId = recipientUserId,
                Chat = chat,
                LastReadMessageId = recipientLastReadMessageId
            });
        }

        await context.SaveChangesAsync(CancellationToken.None);
        return (context, chat);
    }

    [Fact]
    public async Task Handle_RecipientNotFound_ThrowsNotFoundException()
    {
        await using var context = TestApplicationDbContext.Create();
        var command = new CreateOrGetPrivateChatCommand("user-1", "nonexistent");

        var act = () => CreateSut(context).Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*nonexistent*");
    }

    [Fact]
    public async Task Handle_ExistingPrivateChatFound_ReturnsIsNewFalse()
    {
        await using var context = TestApplicationDbContext.Create();
        await SeedUserAndChatAsync(context, "user-1", "user-2");
        var command = new CreateOrGetPrivateChatCommand("user-1", "user-2");

        var result = await CreateSut(context).Handle(command, CancellationToken.None);

        result.IsNew.Should().BeFalse();
        result.ChatId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ExistingChat_ReturnsOtherParticipantLastReadMessageId()
    {
        await using var context = TestApplicationDbContext.Create();
        await SeedUserAndChatAsync(context, "user-1", "user-2", recipientLastReadMessageId: "msg-42");
        var command = new CreateOrGetPrivateChatCommand("user-1", "user-2");

        var result = await CreateSut(context).Handle(command, CancellationToken.None);

        result.OtherParticipantLastReadMessageId.Should().Be("msg-42");
    }

    [Fact]
    public async Task Handle_NoChatExists_CreatesNewChatAndReturnsIsNewTrue()
    {
        await using var context = TestApplicationDbContext.Create();
        context.Users.Add(new User { Id = "user-2", UserName = "user-2_name" });
        await context.SaveChangesAsync(CancellationToken.None);
        var command = new CreateOrGetPrivateChatCommand("user-1", "user-2");

        var result = await CreateSut(context).Handle(command, CancellationToken.None);

        result.IsNew.Should().BeTrue();
        var chatExists = await context.Chats.AnyAsync(c => c.Id == result.ChatId);
        chatExists.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NewChat_AddsTwoParticipantsAndTwoReadStatuses()
    {
        await using var context = TestApplicationDbContext.Create();
        context.Users.Add(new User { Id = "user-2", UserName = "user-2_name" });
        await context.SaveChangesAsync(CancellationToken.None);
        var command = new CreateOrGetPrivateChatCommand("user-1", "user-2");

        var result = await CreateSut(context).Handle(command, CancellationToken.None);

        var participants = await context.ChatParticipants.Where(cp => cp.ChatId == result.ChatId).ToListAsync();
        var readStatuses = await context.ChatReadStatus.Where(crs => crs.ChatId == result.ChatId).ToListAsync();
        participants.Should().HaveCount(2);
        readStatuses.Should().HaveCount(2);
    }
}
