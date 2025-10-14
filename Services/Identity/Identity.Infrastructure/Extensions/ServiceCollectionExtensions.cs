using System.Text;
using Identity.Application.Auth;
using Identity.Application.Email;
using Identity.Infrastructure.Auth;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<JwtOptions>(cfg.GetSection("Jwt"));
        services.Configure<SmtpOptions>(cfg.GetSection("Smtp"));

        services.AddDbContext<IdentityDb>(o =>
            o.UseSqlServer(cfg.GetConnectionString("IdentityDb")));

        var jwt = cfg.GetSection("Jwt").Get<JwtOptions>()!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        var conn = cfg.GetConnectionString("IdentityDb");
        services.AddDbContext<IdentityDb>(opts =>
            opts.UseSqlServer(conn, b => b.MigrationsAssembly("Identity.Infrastructure")));
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IEmailSender, SmtpEmailSender>();

        return services;
    }
}
