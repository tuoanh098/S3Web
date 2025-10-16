using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Student.Application.Services;
using Student.Application.Services.Impl;
using Student.Infrastructure;
using Student.Infrastructure.Services;

using System;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
var env = builder.Environment;

// Logging, controllers, swagger
services.AddLogging();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddScoped<IStudentService, StudentService>();
services.AddStudentInfrastructure(configuration);

var frontendBase = configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";
services.AddCors(options =>
{
    options.AddPolicy("fe", p => p
        .WithOrigins(frontendBase)
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

services.AddHttpClient<IIdentityService, GatewayIdentityService>(client =>
{
    client.BaseAddress = new Uri(configuration["Gateway:BaseUrl"] ?? "http://localhost:7000");
});


var jwtAuthority = configuration["Jwt:Authority"];
var jwtAudience = configuration["Jwt:Audience"];
var jwtKey = configuration["Jwt:Key"];

if (!string.IsNullOrWhiteSpace(jwtAuthority) || !string.IsNullOrWhiteSpace(jwtKey))
{
    // If you use an Authority (IdentityServer/Keycloak/Okta) prefer Authority + Audience
    if (!string.IsNullOrWhiteSpace(jwtAuthority) && !string.IsNullOrWhiteSpace(jwtAudience))
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = jwtAuthority;
                options.Audience = jwtAudience;
                options.RequireHttpsMetadata = false; // set true in production if using https
            });
    }
    else if (!string.IsNullOrWhiteSpace(jwtKey))
    {
        // Symmetric key scenario (simple JWT validation using secret key)
        // Note: for production use proper key management and validation parameters.
        var keyBytes = System.Text.Encoding.UTF8.GetBytes(jwtKey);
        var signingKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // set true in production
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey
                };
            });
    }
}

// Build
var app = builder.Build();

// Dev tools
if (env.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware pipeline
app.UseRouting();


app.UseAuthentication();

app.UseCors("fe");
app.UseAuthorization();

app.MapControllers();

app.Run();
