using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Identity.Application.Auth;
using Identity.Domain.Tokens;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Auth;

public sealed class TokenService(
    UserManager<IdentityUser> users,
    IdentityDb db,
    IOptions<JwtOptions> jwtOpt) : ITokenService
{
    readonly JwtOptions _jwt = jwtOpt.Value;

    public string CreateAccessToken(IdentityUser user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? "")
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

    public async Task<(string token, DateTime expires)> IssueRefreshAsync(string userId)
    {
        var token = Base64UrlEncode(RandomNumberGenerator.GetBytes(64));
        var expires = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays);

        db.RefreshTokens.Add(new RefreshToken
        {
            Token = token,
            UserId = userId,
            ExpiresAt = expires
        });
        await db.SaveChangesAsync();
        return (token, expires);
    }

    public async Task<IdentityUser?> ValidateRefreshAsync(string token)
    {
        var rt = await db.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Token == token);
        if (rt is null || !rt.IsActive) return null;
        return await users.FindByIdAsync(rt.UserId);
    }

    public async Task<(string newToken, DateTime expires)?> RotateAsync(string oldToken)
    {
        var current = await db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == oldToken);
        if (current is null || !current.IsActive) return null;

        current.RevokedAt = DateTime.UtcNow;
        var (newToken, exp) = await IssueRefreshAsync(current.UserId);
        current.ReplacedByToken = newToken;
        await db.SaveChangesAsync();

        return (newToken, exp);
    }

    public async Task RevokeAsync(string token, string? reason = null)
    {
        var rt = await db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        if (rt is null || !rt.IsActive) return;
        rt.RevokedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
}
