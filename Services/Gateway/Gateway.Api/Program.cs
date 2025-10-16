using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var services = builder.Services;

// --- config: load from appsettings
// You should add ServiceUrls:IdentityApi and ServiceUrls:StudentApi and Internal:ServiceToken in appsettings.json

// Add controllers + swagger
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// Register named HttpClients for Identity and Student (Gateway will call them)
services.AddHttpClient("identity", c =>
{

    c.BaseAddress = new Uri(config["ServiceUrls:IdentityApi"] ?? "http://localhost:5001");
    c.Timeout = TimeSpan.FromSeconds(10);
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
services.AddHttpClient("student", c =>
{
    c.BaseAddress = new Uri(config["ServiceUrls:StudentApi"] ?? "http://localhost:5011");
    c.Timeout = TimeSpan.FromSeconds(10);
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddCors(o =>
{
    o.AddPolicy("fe", p => p
        .WithOrigins("http://localhost:5173", "https://yourfe.example.com")
        .AllowAnyHeader().AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("fe");

app.UseAuthorization();

app.MapControllers();

app.Run();
