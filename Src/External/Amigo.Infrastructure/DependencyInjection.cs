using Amigo.Application.Abstraction.Services;
using Amigo.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Amigo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        #region Localization
        services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });

        var supportedCultures = new[]
        {
            new CultureInfo("en"),
            new CultureInfo("es"),
            new CultureInfo("fr"),
            new CultureInfo("it"),
            new CultureInfo("pt"),
            new CultureInfo("pt-BR")
        };

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("en");

            options.SupportedCultures = supportedCultures;

            options.SupportedUICultures = supportedCultures;
        });

        #endregion

        services.AddScoped<ILocalizationService, LocalizationService>();

        return services;
    }
}
