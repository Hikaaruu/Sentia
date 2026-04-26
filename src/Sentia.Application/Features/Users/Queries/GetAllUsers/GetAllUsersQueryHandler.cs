using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Common.Models;
using Sentia.Application.Features.Users.Dtos;

namespace Sentia.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllUsersQuery, PaginatedResponse<UserDto>>
{
    public async Task<PaginatedResponse<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = context.Users
            .Where(u => u.Id != request.CurrentUserId)
            .OrderBy(u => u.UserName);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<UserDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<UserDto>(items, totalCount, request.Page, request.PageSize);
    }
}
