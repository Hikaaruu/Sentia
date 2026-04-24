using MediatR;

namespace Sentia.Application.Features.Auth.Commands.Register;

public record RegisterResult(string UserId, string Username);

public record RegisterCommand(string Username, string Password) : IRequest<RegisterResult>;
