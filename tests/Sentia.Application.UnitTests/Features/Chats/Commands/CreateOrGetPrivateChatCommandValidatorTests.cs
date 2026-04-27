using FluentAssertions;
using Sentia.Application.Features.Chats.Commands.CreateOrGetPrivateChat;

namespace Sentia.Application.UnitTests.Features.Chats.Commands;

public class CreateOrGetPrivateChatCommandValidatorTests
{
    private static CreateOrGetPrivateChatCommandValidator CreateSut() => new();

    [Fact]
    public void Validate_SameCurrentUserAndRecipient_FailsWithCannotChatWithYourselfMessage()
    {
        var command = new CreateOrGetPrivateChatCommand("user-1", "user-1");

        var result = CreateSut().Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == nameof(command.RecipientUserId) &&
            e.ErrorMessage == "Cannot start a chat with yourself.");
    }

    [Fact]
    public void Validate_DifferentUsers_Passes()
    {
        var command = new CreateOrGetPrivateChatCommand("user-1", "user-2");

        var result = CreateSut().Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_RecipientUserIdExceedsMaxLength_Fails()
    {
        var command = new CreateOrGetPrivateChatCommand("user-1", new string('x', 451));

        var result = CreateSut().Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.RecipientUserId));
    }

    [Fact]
    public void Validate_CurrentUserIdExceedsMaxLength_Fails()
    {
        var command = new CreateOrGetPrivateChatCommand(new string('x', 451), "user-2");

        var result = CreateSut().Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.CurrentUserId));
    }
}
