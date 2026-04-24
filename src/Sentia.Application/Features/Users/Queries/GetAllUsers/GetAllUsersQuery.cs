using MediatR;
using Sentia.Application.Features.Users.Dtos;

namespace Sentia.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersResult(List<UserDto> Users);

public record GetAllUsersQuery(
    int Page,
    int PageSize,
    string CurrentUserId) : IRequest<GetAllUsersResult>;
