using Amigo.Application.Abstraction;
using Amigo.Application.Abstraction.MappingInterfaces;
using Amigo.Application.Abstraction.Services;
using Amigo.Application.Abstraction.Services.Authentication;
using Amigo.Application.Mapping;
using Amigo.Application.Services;
using Amigo.Domain.DTO.Destination;
using Amigo.Domain.DTO.Images;
using Amigo.Domain.Extension;
using Amigo.SharedKernal.QueryParams;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Amigo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IUserMapping, UserMapping>();
        services.AddScoped<IDestinationMapping, DestinationMapping>();


        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, GoogleEmailService>();
        services.AddScoped<IEnumService, EnumService>();
        services.AddScoped<IDestinationService , DestinationService>();
        services.AddScoped<IImageService, ImageService>();

        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
        services.AddSingleton<ImageCloudService>();


        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IValidationService, ValidationService>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<ConfirmEmailRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<LoginRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<ResendConfrimEmailRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<ForgetPasswordRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<ResetPasswordRequestDTO>();

        services.AddValidatorsFromAssemblyContaining<CreateDestinationRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<UploadImageRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<GetAllDestinationQuery>();

        services.AddValidatorsFromAssemblyContaining<UpdateActivationDestinationRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<UpdateDestinationRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<GetDestinationByIdQuery>();


        return services;
    }
}
