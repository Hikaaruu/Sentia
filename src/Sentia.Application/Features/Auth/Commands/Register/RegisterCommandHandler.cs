using MediatR;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Auth.Dtos;

namespace Sentia.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(IIdentityService identityService)
    : IRequestHandler<RegisterCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
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

        return new AuthResultDto(userId, request.Username);
    }
}
