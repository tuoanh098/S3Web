// Identity.Infrastructure/Auth/TokenService.cs
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Identity.Application.Auth;
using Identity.Domain.Tokens;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Auth
{
    public sealed class TokenService : ITokenService
    {
        private readonly UserManager<IdentityUser> _users;
        private readonly IdentityDb _db;
        private readonly JwtOptions _jwt;

        public TokenService(
            UserManager<IdentityUser> users,
            IdentityDb db,
            IOptions<JwtOptions> jwtOpt)
        {
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _jwt = jwtOpt?.Value ?? throw new ArgumentNullException(nameof(jwtOpt));
        }

        // ----------------------
        // Access token (JWT)
        // ----------------------
        public string CreateAccessToken(IdentityUser user, IEnumerable<string> roles)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            roles ??= Enumerable.Empty<string>();

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ----------------------
        // Refresh token helpers
        // ----------------------

        // Create a cryptographically secure refresh token string and persist it via RefreshTokens table
        public async Task<string> CreateRefreshTokenAsync(IdentityUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var token = Base64UrlEncode(RandomNumberGenerator.GetBytes(64));
            var expires = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays);

            var entity = new RefreshToken
            {
                Token = token,
                UserId = user.Id,
                ExpiresAt = expires
            };

            _db.RefreshTokens.Add(entity);
            await _db.SaveChangesAsync();

            return token;
        }

        // Validate refresh token for a specific user id
        public async Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(refreshToken))
                return false;

            var rt = await _db.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == refreshToken);

            if (rt == null) return false;
            if (!rt.IsActive) return false;
            if (rt.ExpiresAt < DateTime.UtcNow) return false;

            return true;
        }

        // Issue a new refresh token for a given userId (used by login flow)
        public async Task<(string token, DateTime expires)> IssueRefreshAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

            var token = Base64UrlEncode(RandomNumberGenerator.GetBytes(64));
            var expires = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays);

            var rt = new RefreshToken
            {
                Token = token,
                UserId = userId,
                ExpiresAt = expires
            };

            _db.RefreshTokens.Add(rt);
            await _db.SaveChangesAsync();

            return (token, expires);
        }

        // Validate refresh token and return the IdentityUser if valid (used by refresh endpoint)
        public async Task<IdentityUser?> ValidateRefreshAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            var rt = await _db.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Token == token);

            if (rt == null || !rt.IsActive) return null;

            var user = await _users.FindByIdAsync(rt.UserId);
            return user;
        }

        // Rotate a refresh token: revoke current one, issue a new one and link replaced token
        public async Task<(string newToken, DateTime expires)?> RotateAsync(string oldToken)
        {
            if (string.IsNullOrWhiteSpace(oldToken)) return null;

            var current = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == oldToken);
            if (current == null || !current.IsActive) return null;

            // revoke current
            current.RevokedAt = DateTime.UtcNow;

            // issue a new one
            var issued = await IssueRefreshAsync(current.UserId);
            var newToken = issued.token;
            var expires = issued.expires;

            current.ReplacedByToken = newToken;
            await _db.SaveChangesAsync();

            return (newToken, expires);
        }

        // Revoke a token (logout or admin revoke)
        public async Task RevokeAsync(string token, string? reason = null)
        {
            if (string.IsNullOrWhiteSpace(token)) return;

            var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
            if (rt == null || !rt.IsActive) return;

            rt.RevokedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(reason))
            {
                rt.RevokedReason = reason;
            }

            await _db.SaveChangesAsync();
        }

        // ----------------------
        // Helpers
        // ----------------------
        private static string Base64UrlEncode(byte[] bytes) =>
            Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');

        // Keep this for completeness; if you need a different size change param
        private static string CreateSecureTokenString(int bytes = 64)
        {
            var data = new byte[bytes];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(data);
            var s = Convert.ToBase64String(data);
            s = s.TrimEnd('=').Replace('+', '-').Replace('/', '_');
            return s;
        }
    }
}
