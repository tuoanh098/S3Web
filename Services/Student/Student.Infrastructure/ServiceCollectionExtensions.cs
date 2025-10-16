using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Student.Application.Repositories;
using Student.Infrastructure.Repositories;

namespace Student.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStudentInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DB
            var conn = configuration.GetConnectionString("StudentDb");
            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("Connection string 'StudentDb' is not configured.");

            services.AddDbContext<StudentDbContext>(opts =>
                opts.UseSqlServer(conn, b => b.MigrationsAssembly(typeof(ServiceCollectionExtensions).Assembly.FullName)));

            services.AddScoped<IStudentRepository, StudentRepository>();

            return services;
        }
    }
}
