using FluentAssertions;
using Sentia.Application.Features.Chats.Commands.MarkChatAsRead;

namespace Sentia.Application.UnitTests.Features.Chats.Commands;

public class MarkChatAsReadCommandValidatorTests
{
    private static MarkChatAsReadCommandValidator CreateSut() => new();

    [Fact]
    public void Validate_ChatIdOfZero_Fails()
    {
        var command = new MarkChatAsReadCommand(ChatId: 0, CurrentUserId: "user-1", MessageId: "msg-1");

        var result = CreateSut().Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(command.ChatId) &&
            e.ErrorMessage == "ChatId must be a valid ID.");
    }

    [Fact]
    public void Validate_MessageIdExceedsMaxLength_Fails()
    {
        var command = new MarkChatAsReadCommand(ChatId: 1, CurrentUserId: "user-1", MessageId: new string('x', 41));

        var result = CreateSut().Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.MessageId));
    }
}
