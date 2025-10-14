using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Student.Infrastructure;
using Student.Application;

var builder = WebApplication.CreateBuilder(args);

// add configuration and logging
builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register application + infrastructure
builder.Services.AddStudentApplication();
builder.Services.AddStudentInfrastructure(builder.Configuration);

// CORS (adjust allowed origins)
builder.Services.AddCors(options =>
{
    options.AddPolicy("fe", p => p
        .WithOrigins(builder.Configuration["Frontend:BaseUrl"] ?? "http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

// build
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// middleware
app.UseRouting();
app.UseCors("fe");
app.UseAuthorization();

app.MapControllers();


app.Run();
