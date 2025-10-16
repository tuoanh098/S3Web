using Microsoft.AspNetCore.Identity;
using Student.Application.Services;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser> _users;
    public IdentityService(UserManager<IdentityUser> users) => _users = users;

    public async Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var u = await _users.FindByIdAsync(userId);
        return u?.Email;
    }
}
