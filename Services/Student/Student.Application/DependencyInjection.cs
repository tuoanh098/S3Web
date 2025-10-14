using Microsoft.Extensions.DependencyInjection;
using Student.Application.Interfaces;

namespace Student.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddStudentApplication(this IServiceCollection services)
    {
        return services;
    }
}
