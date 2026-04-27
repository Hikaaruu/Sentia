using FluentAssertions;
using Moq;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Auth.Commands.Login;

namespace Sentia.Application.UnitTests.Features.Auth.Commands;

public class LoginCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();

    private LoginCommandHandler CreateSut() => new(_identityService.Object);

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsLoginResult()
    {
        const string userId = "user-1";
        const string username = "alice";
        _identityService
            .Setup(s => s.ValidateCredentialsAsync(username, "P@ssw0rd"))
            .ReturnsAsync((true, userId));
        var command = new LoginCommand(username, "P@ssw0rd");

        var result = await CreateSut().Handle(command, CancellationToken.None);

        result.UserId.Should().Be(userId);
        result.Username.Should().Be(username);
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ThrowsValidationException()
    {
        _identityService
            .Setup(s => s.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((false, string.Empty));
        var command = new LoginCommand("alice", "wrong");

        var act = () => CreateSut().Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*validation*");
    }
}
