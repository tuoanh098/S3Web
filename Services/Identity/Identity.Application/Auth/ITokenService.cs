using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Auth;

public interface ITokenService
{
    string CreateAccessToken(IdentityUser user, IEnumerable<string> roles);
    Task<(string token, DateTime expires)> IssueRefreshAsync(string userId);
    Task<IdentityUser?> ValidateRefreshAsync(string token);
    Task<(string newToken, DateTime expires)?> RotateAsync(string oldToken);
    Task RevokeAsync(string token, string? reason = null);
}
