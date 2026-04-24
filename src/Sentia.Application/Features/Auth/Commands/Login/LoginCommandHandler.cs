using MediatR;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Auth.Dtos;

namespace Sentia.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IIdentityService identityService)
    : IRequestHandler<LoginCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (success, userId) = await identityService.ValidateCredentialsAsync(
            request.Username,
            request.Password);

        if (!success)
            throw new ValidationException("Credentials", "Invalid username or password.");

        return new AuthResultDto(userId, request.Username);
    }
}
