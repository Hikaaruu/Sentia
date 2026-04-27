using FluentAssertions;
using Moq;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Dtos;
using Sentia.Application.Features.Messages.EventHandlers;
using Sentia.Application.Features.Messages.Events;

namespace Sentia.Application.UnitTests.Features.Messages.EventHandlers;

public class BroadcastMessageEventHandlerTests
{
    private readonly Mock<ISignalRService> _signalRService = new();

    private BroadcastMessageEventHandler CreateSut() => new(_signalRService.Object);

    private static MessageCreatedEvent CreateEvent() => new(
        MessageId: "msg-1",
        ChatId: 10,
        SenderId: "user-1",
        Content: "Hello world",
        CreatedAt: new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc),
        ParticipantUserIds: ["user-1", "user-2"]);

    [Fact]
    public async Task Handle_ValidEvent_CallsBroadcastNewMessageWithCorrectPayload()
    {
        _signalRService
            .Setup(s => s.BroadcastNewMessageAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<NewMessagePayload>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var notification = CreateEvent();

        await CreateSut().Handle(notification, CancellationToken.None);

        _signalRService.Verify(s => s.BroadcastNewMessageAsync(
            notification.ParticipantUserIds,
            It.Is<NewMessagePayload>(p =>
                p.MessageId == notification.MessageId &&
                p.ChatId == notification.ChatId &&
                p.SenderId == notification.SenderId &&
                p.Content == notification.Content &&
                p.CreatedAt == notification.CreatedAt),
            CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidEvent_PassesCancellationTokenNone()
    {
        _signalRService
            .Setup(s => s.BroadcastNewMessageAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<NewMessagePayload>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await CreateSut().Handle(CreateEvent(), CancellationToken.None);

        _signalRService.Verify(
            s => s.BroadcastNewMessageAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<NewMessagePayload>(), CancellationToken.None),
            Times.Once);
    }
}
