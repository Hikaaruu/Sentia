using MediatR;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;

namespace Sentia.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(IIdentityService identityService)
    : IRequestHandler<RegisterCommand, RegisterResult>
{
    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (success, userId, errors) = await identityService.CreateUserAsync(
            request.Username,
            request.Password);

        if (!success)
        {
            var errorDict = errors
                .GroupBy(_ => "Username")
                .ToDictionary(g => g.Key, g => g.ToArray());

            throw new ValidationException(errorDict);
        }

        return new RegisterResult(userId, request.Username);
    }
}
