using Identity.Application.Email;
using Identity.Infrastructure.Extensions;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Identity (you already have this configured)
builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<Identity.Infrastructure.Persistence.IdentityDb>()
    .AddDefaultTokenProviders();

// Infrastructure (EF, JWT, Email, TokenService)
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

var conn = builder.Configuration.GetConnectionString("IdentityDb");
if (string.IsNullOrWhiteSpace(conn))
{
    throw new InvalidOperationException("Connection string 'IdentityDb' not found. Check appsettings or environment variables.");
}

builder.Services.AddDbContext<IdentityDb>(opts =>
    opts.UseSqlServer(conn, b => b.MigrationsAssembly("Identity.Infrastructure")));

// CORS locked to FE origin(s)
builder.Services.AddCors(o =>
{
    o.AddPolicy("fe", p => p
        .WithOrigins("http://localhost:5173", "https://yourfe.example.com")
        .AllowAnyHeader().AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();
app.UseSwagger(); app.UseSwaggerUI();

//// run seeding (async)
//await app.Services.EnsureSeedDataAsync(app.Environment);

var endpoints = app.Services.GetRequiredService<Microsoft.AspNetCore.Routing.EndpointDataSource>();
foreach (var ep in endpoints.Endpoints)
    Console.WriteLine(ep.DisplayName);

app.UseRouting();
app.UseCors("fe");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
