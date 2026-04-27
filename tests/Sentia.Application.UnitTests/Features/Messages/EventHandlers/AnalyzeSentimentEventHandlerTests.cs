using FluentAssertions;
using Moq;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.EventHandlers;
using Sentia.Application.Features.Messages.Events;

namespace Sentia.Application.UnitTests.Features.Messages.EventHandlers;

public class AnalyzeSentimentEventHandlerTests
{
    [Fact]
    public async Task Handle_ValidEvent_QueuesEventOnce()
    {
        var queue = new Mock<ISentimentAnalysisQueue>();
        queue.Setup(q => q.QueueAsync(It.IsAny<MessageCreatedEvent>())).Returns(ValueTask.CompletedTask);
        var sut = new AnalyzeSentimentEventHandler(queue.Object);
        var notification = new MessageCreatedEvent(
            MessageId: "msg-1",
            ChatId: 10,
            SenderId: "user-1",
            Content: "Hello",
            CreatedAt: DateTime.UtcNow,
            ParticipantUserIds: ["user-1", "user-2"]);

        await sut.Handle(notification, CancellationToken.None);

        queue.Verify(q => q.QueueAsync(notification), Times.Once);
    }
}
