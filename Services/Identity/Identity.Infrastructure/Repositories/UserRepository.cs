using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Identity.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _users;
        private readonly ILogger<UserRepository> _logger;

        private const string LoginProvider = "AppRefresh";
        private const string TokenName = "RefreshToken";

        public UserRepository(UserManager<IdentityUser> users, ILogger<UserRepository> logger)
        {
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _logger = logger;
        }

        public async Task<IdentityUser?> ValidateCredentialsAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            // Accept username or email
            var user = await _users.FindByNameAsync(username) ?? await _users.FindByEmailAsync(username);
            if (user == null) return null;

            var ok = await _users.CheckPasswordAsync(user, password);
            return ok ? user : null;
        }

        public async Task<IdentityUser?> GetByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;
            return await _users.FindByIdAsync(userId);
        }

        public async Task<IdentityUser?> GetByUsernameAsync(string usernameOrEmail)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail)) return null;
            return await _users.FindByNameAsync(usernameOrEmail) ?? await _users.FindByEmailAsync(usernameOrEmail);
        }

        public async Task SetRefreshTokenAsync(IdentityUser user, string refreshToken, DateTimeOffset expires)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(refreshToken)) throw new ArgumentNullException(nameof(refreshToken));

            // We store value as: "{token}|{expiryTicks}"
            var payload = $"{refreshToken}|{expires.UtcTicks}";
            var result = await _users.SetAuthenticationTokenAsync(user, LoginProvider, TokenName, payload);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to set refresh token for user {UserId}: {Errors}", user.Id, string.Join(";", result.Errors));
                throw new InvalidOperationException("Failed to persist refresh token");
            }
        }

        public async Task<(string? token, DateTimeOffset? expiry)?> GetRefreshTokenAsync(IdentityUser user)
        {
            if (user == null) return null;
            var payload = await _users.GetAuthenticationTokenAsync(user, LoginProvider, TokenName);
            if (string.IsNullOrEmpty(payload)) return null;

            try
            {
                var idx = payload.LastIndexOf('|');
                if (idx <= 0) return null;
                var token = payload.Substring(0, idx);
                var ticksPart = payload.Substring(idx + 1);
                if (!long.TryParse(ticksPart, out var ticks)) return null;
                var expiry = new DateTimeOffset(ticks, TimeSpan.Zero);
                return (token, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse stored refresh token payload for user {UserId}", user.Id);
                return null;
            }
        }

        public async Task<bool> RemoveRefreshTokenAsync(IdentityUser user)
        {
            if (user == null) return false;
            var result = await _users.RemoveAuthenticationTokenAsync(user, LoginProvider, TokenName);
            return result.Succeeded;
        }
    }
}
