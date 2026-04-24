using MediatR;

namespace Sentia.Application.Features.Auth.Commands.Login;

public record LoginResult(string UserId, string Username);

public record LoginCommand(string Username, string Password) : IRequest<LoginResult>;
