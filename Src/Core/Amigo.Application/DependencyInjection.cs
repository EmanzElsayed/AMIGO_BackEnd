using Amigo.Application.Abstraction;
using Amigo.Application.Abstraction.MappingInterfaces;
using Amigo.Application.Abstraction.Services;
using Amigo.Application.Abstraction.Services.Authentication;
using Amigo.Application.Mapping;
using Amigo.Application.Services;
using Amigo.Application.Services.Admin;
using Amigo.Application.Services.Translation;
using Amigo.Domain.DTO.AvailableSlots;
using Amigo.Domain.DTO.Cancellation;
using Amigo.Domain.DTO.Destination;
using Amigo.Domain.DTO.Images;
using Amigo.Domain.DTO.Price;
using Amigo.Domain.DTO.Tour;
using Amigo.Domain.DTO.TourSchedule;
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
        // Mapping

        services.AddScoped<IUserMapping, UserMapping>();
        services.AddScoped<IDestinationMapping, DestinationMapping>();
        services.AddScoped<ITourMapping, TourMapping>();
        services.AddScoped<IImageMapping, ImageMapping>();
        services.AddScoped<IPriceMapping, PriceMapping>();
        services.AddScoped<ITourScheduleMapping, TourScheduleMapping>();
        services.AddScoped<IAvailableSlotsMapping, AvailableSlotsMapping>();
        services.AddScoped<IIncludeMapping, IncludeMapping>();
        services.AddScoped<INotIncludedMapping, NotIncludedMapping>();
        services.AddScoped<ICancellationMapping, CancellationMapping>();

        //Services

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, GoogleEmailService>();
        services.AddScoped<IEnumService, EnumService>();
        services.AddScoped<IDestinationService , DestinationService>();
        
        services.AddScoped<IAdminDestinationService, AdminDestinationService>();
        services.AddScoped<IAdminTourService, AdminTourService>();
        services.AddScoped<IAdminPriceService, AdminPriceService>();
        services.AddScoped<IAdminAvailableSlotsService, AdminAvailableSlotsService>();
        services.AddScoped<IAdminTourScheduleService, AdminTourScheduleService>();
        services.AddScoped<IAdminTourIncludesService, AdminTourIncludesService>();
        services.AddScoped<IAdminTourNotIncludesService, AdminTourNotIncludesService>();
        services.AddScoped<IAdminTourCancellationService, AdminTourCancellationService>();




        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<TranslationService>();
        services.AddHttpClient<TranslationService>();
        services.AddScoped<TranslationEngine>();

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
        services.AddValidatorsFromAssemblyContaining<UploadMultiImagesRequestDTO>();

        services.AddValidatorsFromAssemblyContaining<GetAllDestinationQuery>();

        services.AddValidatorsFromAssemblyContaining<UpdateActivationDestinationRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<UpdateDestinationRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<GetDestinationByIdQuery>();

        services.AddValidatorsFromAssemblyContaining<CreateTourRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<ImageUrlsRequestDTO>();


        services.AddValidatorsFromAssemblyContaining<CreatePriceRequestDTO>();


        services.AddValidatorsFromAssemblyContaining<CreateTourScheduleRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<CreateAvailableSlotsRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<CreateCancellationRequestDTO>();



        return services;
    }
}
