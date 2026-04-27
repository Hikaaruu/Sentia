using FluentAssertions;
using Moq;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Dtos;
using Sentia.Application.Features.Messages.EventHandlers;
using Sentia.Application.Features.Messages.Events;

namespace Sentia.Application.UnitTests.Features.Messages.EventHandlers;

public class BroadcastReadReceiptEventHandlerTests
{
    private readonly Mock<ISignalRService> _signalRService = new();

    private BroadcastReadReceiptEventHandler CreateSut() => new(_signalRService.Object);

    private static MessageReadEvent CreateEvent() => new(
        MessageId: "msg-1",
        ChatId: 10,
        SenderId: "user-1",
        ReadByUserId: "user-2");

    [Fact]
    public async Task Handle_ValidEvent_CallsBroadcastReadReceiptWithCorrectPayload()
    {
        _signalRService
            .Setup(s => s.BroadcastReadReceiptAsync(It.IsAny<string>(), It.IsAny<ReadReceiptPayload>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var notification = CreateEvent();

        await CreateSut().Handle(notification, CancellationToken.None);

        _signalRService.Verify(s => s.BroadcastReadReceiptAsync(
            notification.SenderId,
            It.Is<ReadReceiptPayload>(p =>
                p.MessageId == notification.MessageId &&
                p.ChatId == notification.ChatId &&
                p.ReadByUserId == notification.ReadByUserId),
            CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidEvent_SendsToOriginalMessageSender()
    {
        _signalRService
            .Setup(s => s.BroadcastReadReceiptAsync(It.IsAny<string>(), It.IsAny<ReadReceiptPayload>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var notification = CreateEvent();

        await CreateSut().Handle(notification, CancellationToken.None);

        _signalRService.Verify(
            s => s.BroadcastReadReceiptAsync(notification.SenderId, It.IsAny<ReadReceiptPayload>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
