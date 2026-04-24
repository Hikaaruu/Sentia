using MediatR;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;

namespace Sentia.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IIdentityService identityService)
    : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (success, userId) = await identityService.ValidateCredentialsAsync(
            request.Username,
            request.Password);

        if (!success)
            throw new ValidationException("Credentials", "Invalid username or password.");

        return new LoginResult(userId, request.Username);
    }
}
