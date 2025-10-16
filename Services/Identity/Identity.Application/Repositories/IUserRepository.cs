using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Repositories
{
    public interface IUserRepository
    {
        /// <summary>Validate username/password and return IdentityUser if ok; otherwise null.</summary>
        Task<IdentityUser?> ValidateCredentialsAsync(string username, string password);

        /// <summary>Get user by Id.</summary>
        Task<IdentityUser?> GetByIdAsync(string userId);

        /// <summary>Get user by username or email.</summary>
        Task<IdentityUser?> GetByUsernameAsync(string usernameOrEmail);

        /// <summary>Store refresh token and expiry for the user.</summary>
        Task SetRefreshTokenAsync(IdentityUser user, string refreshToken, System.DateTimeOffset expires);

        /// <summary>Get stored refresh token value and its expiry (token, expiry) or null if none.</summary>
        Task<(string? token, System.DateTimeOffset? expiry)?> GetRefreshTokenAsync(IdentityUser user);

        /// <summary>Remove stored refresh token for user.</summary>
        Task<bool> RemoveRefreshTokenAsync(IdentityUser user);
    }
}
