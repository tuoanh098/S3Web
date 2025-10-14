using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using Students.Api.Infrastructure;

var logger = NLog.LogManager.Setup()
    .LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // NLog as the only logger
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // EF Core
    builder.Services.AddDbContext<StudentsDb>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // basic health check
    app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "Students.Api" }));

    // sample DB ping (helps to test after migration)
    app.MapGet("/db-ping", async (StudentsDb db) => await db.Database.CanConnectAsync());

    app.Run();
}
catch (Exception ex)
{
    // NLog: catch setup errors
    logger.Error(ex, "Stopped program because of excepti    on");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
