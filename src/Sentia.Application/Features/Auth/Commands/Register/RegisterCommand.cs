using MediatR;
using Sentia.Application.Features.Auth.Dtos;

namespace Sentia.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Username, string Password) : IRequest<AuthResultDto>;
