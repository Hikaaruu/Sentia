namespace Sentia.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Success, string UserId, string[] Errors)> CreateUserAsync(
        string username,
        string password);

    Task<(bool Success, string UserId)> ValidateCredentialsAsync(
        string username,
        string password);

    Task<bool> UserExistsAsync(string userId);
}
