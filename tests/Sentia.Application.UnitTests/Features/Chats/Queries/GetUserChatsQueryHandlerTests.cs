using FluentAssertions;
using Sentia.Application.Features.Chats.Dtos;
using Sentia.Application.Features.Chats.Queries.GetUserChats;
using Sentia.Application.Common.Interfaces;
using Moq;

namespace Sentia.Application.UnitTests.Features.Chats.Queries;

public class GetUserChatsQueryHandlerTests
{
    private readonly Mock<IChatQueryService> _chatQueryService = new();

    private GetUserChatsQueryHandler CreateSut() => new(_chatQueryService.Object);

    [Fact]
    public async Task Handle_ValidRequest_ReturnsChatListFromService()
    {
        var chats = new List<ChatSummaryDto>
        {
            new(1, "user-2", "alice", DateTime.UtcNow, "Hey", "user-2", 0, null),
            new(2, "user-3", "bob", DateTime.UtcNow, "Hi", "user-3", 1, null)
        };
        _chatQueryService
            .Setup(s => s.GetUserChatsAsync("user-1", CancellationToken.None))
            .ReturnsAsync(chats);
        var query = new GetUserChatsQuery("user-1");

        var result = await CreateSut().Handle(query, CancellationToken.None);

        result.Chats.Should().BeEquivalentTo(chats);
    }

    [Fact]
    public async Task Handle_EmptyServiceResult_ReturnsEmptyList()
    {
        _chatQueryService
            .Setup(s => s.GetUserChatsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var query = new GetUserChatsQuery("user-1");

        var result = await CreateSut().Handle(query, CancellationToken.None);

        result.Chats.Should().BeEmpty();
    }
}
