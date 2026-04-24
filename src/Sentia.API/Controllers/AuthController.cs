using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentia.API.Services;
using Sentia.Application.Features.Auth.Commands.Login;
using Sentia.Application.Features.Auth.Commands.Register;

namespace Sentia.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(ISender sender, JwtService jwtService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RegisterCommand(request.Username, request.Password), cancellationToken);
        var token = jwtService.GenerateToken(result.UserId, result.Username);
        return Ok(new { token, result.UserId, result.Username });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new LoginCommand(request.Username, request.Password), cancellationToken);
        var token = jwtService.GenerateToken(result.UserId, result.Username);
        return Ok(new { token, result.UserId, result.Username });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var username = User.FindFirstValue(ClaimTypes.Name)!;
        return Ok(new { userId, username });
    }
}

public record RegisterRequest(string Username, string Password);
public record LoginRequest(string Username, string Password);
