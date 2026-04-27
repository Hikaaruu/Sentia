using FluentAssertions;
using Moq;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Auth.Commands.Register;

namespace Sentia.Application.UnitTests.Features.Auth.Commands;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();

    private RegisterCommandHandler CreateSut() => new(_identityService.Object);

    [Fact]
    public async Task Handle_RegistrationSucceeds_ReturnsRegisterResult()
    {
        const string userId = "user-1";
        const string username = "alice";
        _identityService
            .Setup(s => s.CreateUserAsync(username, "P@ssw0rd"))
            .ReturnsAsync((true, userId, Array.Empty<string>()));
        var command = new RegisterCommand(username, "P@ssw0rd");

        var result = await CreateSut().Handle(command, CancellationToken.None);

        result.UserId.Should().Be(userId);
        result.Username.Should().Be(username);
    }

    [Fact]
    public async Task Handle_RegistrationFails_ThrowsValidationException()
    {
        _identityService
            .Setup(s => s.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((false, string.Empty, new[] { "Username already taken." }));
        var command = new RegisterCommand("bob", "P@ssw0rd");

        var act = () => CreateSut().Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
        _identityService.Verify(s => s.CreateUserAsync("bob", "P@ssw0rd"), Times.Once);
    }

    [Fact]
    public async Task Handle_RegistrationFails_ErrorsGroupedUnderUsernameKey()
    {
        var errors = new[] { "Username already taken.", "Username is invalid." };
        _identityService
            .Setup(s => s.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((false, string.Empty, errors));
        var command = new RegisterCommand("bob", "P@ssw0rd");

        var act = () => CreateSut().Handle(command, CancellationToken.None);

        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().ContainKey("Username")
            .WhoseValue.Should().BeEquivalentTo(errors);
    }
}
