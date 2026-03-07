using Amigo.Application.Abstraction.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amigo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITourService, ITourService>();
        return services;
    }
}
