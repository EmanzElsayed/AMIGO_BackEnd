using Amigo.Application.Abstraction.Services;
using Amigo.Domain.Abstraction;
using Amigo.Domain.Abstraction.Repositories;
using Amigo.Persistence.Repositories;
using Amigo.Persistence.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace Amigo.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AmigoDbContext>(options =>
            options.UseNpgsql(connectionString, options =>
            {
                options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(15),
                    errorCodesToAdd: null
                );
            }));
        services.AddDataProtection();


        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
        }).AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AmigoDbContext>()
        .AddSignInManager()
        .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);

        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromMinutes(30);
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 3;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
        });

        services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();

        services.AddScoped<IDataSeeding, DataSeeding>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        //services.AddScoped<ITopDestinationsReader, TopDestinationsReader>();
        services.AddScoped<IDestinationSlugResolver, DestinationSlugResolver>();
        //services.AddScoped<ITourReviewEligibilityReader, TourReviewEligibilityReader>();
        services.AddScoped<IUserRepo, UserRepo>();

        services.AddScoped<ISlotsRepo, SlotsRepo>();

        return services;
    }
}
