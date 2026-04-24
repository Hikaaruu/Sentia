using MediatR;
using Sentia.Application.Features.Users.Dtos;

namespace Sentia.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery(
    int Page,
    int PageSize,
    string CurrentUserId) : IRequest<List<UserDto>>;
