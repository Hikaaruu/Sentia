using MediatR;
using Sentia.Application.Features.Auth.Dtos;

namespace Sentia.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Username, string Password) : IRequest<AuthResultDto>;
