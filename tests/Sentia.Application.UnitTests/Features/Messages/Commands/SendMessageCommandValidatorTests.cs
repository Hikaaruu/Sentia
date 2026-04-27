using FluentAssertions;
using Sentia.Application.Features.Messages.Commands.SendMessage;

namespace Sentia.Application.UnitTests.Features.Messages.Commands;

public class SendMessageCommandValidatorTests
{
    private static SendMessageCommandValidator CreateSut() => new();

    [Fact]
    public void Validate_ContentAtExactMaxLength_Passes()
    {
        var command = new SendMessageCommand("msg-1", ChatId: 1, SenderId: "user-1", Content: new string('a', 4000));

        var result = CreateSut().Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ContentExceedsMaxLength_Fails()
    {
        var command = new SendMessageCommand("msg-1", ChatId: 1, SenderId: "user-1", Content: new string('a', 4001));

        var result = CreateSut().Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(command.Content) &&
            e.ErrorMessage == "Message content must not exceed 4000 characters.");
    }

    [Fact]
    public void Validate_EmptyContent_FailsWithContentCannotBeEmptyMessage()
    {
        var command = new SendMessageCommand("msg-1", ChatId: 1, SenderId: "user-1", Content: "");

        var result = CreateSut().Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(command.Content) &&
            e.ErrorMessage == "Message content cannot be empty.");
    }

    [Fact]
    public void Validate_MessageIdExceedsMaxLength_Fails()
    {
        var command = new SendMessageCommand(new string('x', 41), ChatId: 1, SenderId: "user-1", Content: "Hello");

        var result = CreateSut().Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.MessageId));
    }
}
