using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentia.Application.Features.Users.Queries.GetAllUsers;

namespace Sentia.API.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await sender.Send(
            new GetAllUsersQuery(page, pageSize, currentUserId),
            cancellationToken);

        return Ok(result.Users);
    }
}
