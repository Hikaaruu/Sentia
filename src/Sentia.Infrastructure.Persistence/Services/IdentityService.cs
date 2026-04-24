using Microsoft.AspNetCore.Identity;
using Sentia.Application.Common.Interfaces;

namespace Sentia.Infrastructure.Persistence.Services;

public class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    public async Task<(bool Success, string UserId, string[] Errors)> CreateUserAsync(
        string username,
        string password)
    {
        var user = new ApplicationUser { UserName = username };
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return (false, string.Empty, errors);
        }

        return (true, user.Id, []);
    }

    public async Task<(bool Success, string UserId)> ValidateCredentialsAsync(
        string username,
        string password)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user is null)
            return (false, string.Empty);

        var valid = await userManager.CheckPasswordAsync(user, password);
        return valid ? (true, user.Id) : (false, string.Empty);
    }

    public async Task<bool> UserExistsAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user is not null;
    }
}
