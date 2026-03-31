using Amigo.Application.Abstraction;
using Amigo.Application.Abstraction.Services;
using Amigo.Application.Abstraction.Services.Authentication;
using Amigo.Application.Services;
using Amigo.Domain.Extension;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Amigo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IUserMapping, UserMapping>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, GoogleEmailService>();

        services.AddScoped<IEnumService, EnumService>();

        services.AddScoped<IValidationService, ValidationService>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<ConfirmEmailRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<LoginRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<ResendConfrimEmailRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<ForgetPasswordRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<ResetPasswordRequestDTO>();


        return services;
    }
}
