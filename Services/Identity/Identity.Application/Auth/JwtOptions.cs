﻿namespace Identity.Application.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string Key { get; init; } = default!;
    public int AccessTokenMinutes { get; init; } = 60;
    public int RefreshTokenDays { get; init; } = 7;
}