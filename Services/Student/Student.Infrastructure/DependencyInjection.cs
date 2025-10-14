using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Student.Application.Interfaces;
using Student.Infrastructure.Persistence;
using Student.Infrastructure.Repositories;
using Student.Infrastructure.Services;

namespace Student.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStudentInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("StudentDb");
        services.AddDbContext<StudentDbContext>(opts =>
            opts.UseSqlServer(conn, b => b.MigrationsAssembly("Student.Infrastructure")));

        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IStudentService, StudentService>();

        return services;
    }
}
