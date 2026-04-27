using FluentAssertions;
using Moq;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Commands.ProcessMessageSentiment;
using Sentia.Application.Features.Messages.Dtos;
using Sentia.Application.UnitTests.Infrastructure;
using Sentia.Domain.Entities;

namespace Sentia.Application.UnitTests.Features.Messages.Commands;

public class ProcessMessageSentimentCommandHandlerTests
{
    private readonly Mock<ISentimentAnalysisService> _sentimentService = new();
    private readonly Mock<ISignalRService> _signalRService = new();

    private ProcessMessageSentimentCommandHandler CreateSut(TestApplicationDbContext context)
        => new(context, _sentimentService.Object, _signalRService.Object);

    private static ProcessMessageSentimentCommand CreateCommand(string messageId = "msg-1") => new(
        MessageId: messageId,
        ChatId: 10,
        Content: "I feel great today",
        ParticipantUserIds: ["user-1", "user-2"]);

    [Fact]
    public async Task Handle_MessageNotFoundInDb_CompletesWithoutSavingOrBroadcasting()
    {
        await using var context = TestApplicationDbContext.Create();
        _sentimentService
            .Setup(s => s.AnalyzeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SentimentResult(SentimentLabel.Positive, 0.95));

        await CreateSut(context).Handle(CreateCommand("nonexistent"), CancellationToken.None);

        _signalRService.Verify(
            s => s.BroadcastSentimentUpdateAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<SentimentUpdatePayload>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_MessageFound_UpdatesSentimentLabelAndScore()
    {
        await using var context = TestApplicationDbContext.Create();
        context.Messages.Add(new Message { Id = "msg-1", ChatId = 10, SenderId = "user-1", Content = "I feel great today", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync(CancellationToken.None);
        _sentimentService
            .Setup(s => s.AnalyzeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SentimentResult(SentimentLabel.Positive, 0.92));
        _signalRService
            .Setup(s => s.BroadcastSentimentUpdateAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<SentimentUpdatePayload>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await CreateSut(context).Handle(CreateCommand("msg-1"), CancellationToken.None);

        var message = await context.Messages.FindAsync(["msg-1"]);
        message!.SentimentLabel.Should().Be(SentimentLabel.Positive);
        message.SentimentScore.Should().BeApproximately(0.92, 0.001);
    }

    [Fact]
    public async Task Handle_MessageFound_BroadcastsSentimentUpdateWithCorrectPayload()
    {
        await using var context = TestApplicationDbContext.Create();
        context.Messages.Add(new Message { Id = "msg-1", ChatId = 10, SenderId = "user-1", Content = "I feel great today", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync(CancellationToken.None);
        _sentimentService
            .Setup(s => s.AnalyzeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SentimentResult(SentimentLabel.Positive, 0.92));
        _signalRService
            .Setup(s => s.BroadcastSentimentUpdateAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<SentimentUpdatePayload>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var command = CreateCommand("msg-1");

        await CreateSut(context).Handle(command, CancellationToken.None);

        _signalRService.Verify(s => s.BroadcastSentimentUpdateAsync(
            command.ParticipantUserIds,
            It.Is<SentimentUpdatePayload>(p =>
                p.MessageId == "msg-1" &&
                p.ChatId == 10 &&
                p.SentimentLabel == SentimentLabel.Positive &&
                Math.Abs(p.SentimentScore - 0.92) < 0.001),
            CancellationToken.None),
            Times.Once);
    }
}
