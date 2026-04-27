using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Sentia.API.Infrastructure;
using Sentia.API.Services;
using Sentia.Application.Features.Auth.Commands.Login;
using Sentia.Application.Features.Auth.Commands.Register;

namespace Sentia.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(ISender sender, JwtService jwtService) : ControllerBase
{
    [HttpPost("register")]
    [EnableRateLimiting(RateLimitingServiceExtensions.RegisterPolicy)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RegisterCommand(request.Username, request.Password), cancellationToken);
        var token = jwtService.GenerateToken(result.UserId, result.Username);
        return Ok(new AuthResponse(token, result.UserId, result.Username));
    }

    [HttpPost("login")]
    [EnableRateLimiting(RateLimitingServiceExtensions.LoginPolicy)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new LoginCommand(request.Username, request.Password), cancellationToken);
        var token = jwtService.GenerateToken(result.UserId, result.Username);
        return Ok(new AuthResponse(token, result.UserId, result.Username));
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<MeResponse> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var username = User.FindFirstValue(ClaimTypes.Name)!;
        return Ok(new MeResponse(userId, username));
    }
}

public record RegisterRequest(string Username, string Password);
public record LoginRequest(string Username, string Password);
public record AuthResponse(string Token, string UserId, string Username);
public record MeResponse(string UserId, string Username);
