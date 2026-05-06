using Amigo.Application.Abstraction;
using Amigo.Application.Abstraction.MappingInterfaces;
using Amigo.Application.Abstraction.Services;
using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Application.Abstraction.Services.Authentication;
using Amigo.Application.Mapping;
using Amigo.Application.Services;
using Amigo.Application.Services.Admin;
using Amigo.Application.Services.AutoTranslation;
using Amigo.Application.Validators.Checkout;
using Amigo.Application.Validators.CountryInfo;
using Amigo.Application.Validators.Tour;
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
using Microsoft.Extensions.Options;
using PayPalCheckoutSdk.Core;


namespace Amigo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        // Mapping

        services.AddScoped<IDestinationMapping, DestinationMapping>();
        services.AddScoped<ITourMapping, TourMapping>();
        services.AddScoped<IImageMapping, ImageMapping>();
        services.AddScoped<IPriceMapping, PriceMapping>();
        services.AddScoped<ITourScheduleMapping, TourScheduleMapping>();
        services.AddScoped<IAvailableSlotsMapping, AvailableSlotsMapping>();
        services.AddScoped<IInclusionMapping, InclusionMapping>();
        services.AddScoped<ICancellationMapping, CancellationMapping>();

        //Services

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEnumService, EnumService>();
        services.AddScoped<IDestinationService , DestinationService>();
        services.AddScoped<IUserTourCatalogService, UserTourCatalogService>();
        //services.AddScoped<IUserTourReviewService, UserTourReviewService>();

        //services.AddScoped<ITopDestinationsReader, Topde>();

        services.AddScoped<IAdminDestinationService, AdminDestinationService>();
        services.AddScoped<IAdminTourService, AdminTourService>();
        services.AddScoped<IAdminPriceService, AdminPriceService>();
        services.AddScoped<IAdminAvailableSlotsService, AdminAvailableSlotsService>();
        services.AddScoped<IAdminTourScheduleService, AdminTourScheduleService>();

        services.AddScoped<IAdminTourInclusionService, AdminTourInclusionService>();
        services.AddScoped<IAdminTourCancellationService, AdminTourCancellationService>();
        
        services.AddScoped<IAdminCustomerService, AdminCustomerService>();

        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICountryInfoService, CountryInfoService>();

        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IPhoneCodeService, PhoneCodeService>();
        services.AddScoped<TranslationService>();
        services.AddHttpClient<TranslationService>();
        services.AddScoped<TranslationEngine>();

        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IPaymentOrchestrator, PaymentOrchestrator>();

        services.AddScoped<IJWTTokenService, JWTTokenService>();

        services.AddScoped<ITravelersService, TravelersService>();

        services.AddScoped<IPaymentProviderResolver, PaymentProviderResolver>();

        services.AddScoped<IPaymentProvider, StripePaymentProvider>();
        services.AddScoped<IPaymentProvider, PaypalPaymentProvider>();
        services.AddScoped<IPaymentService, PaymentService>();

        services.AddScoped<ICheckoutQuoteService, CheckoutQuoteService>();

        //services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IBookingService, BookingService>();

        services.AddSingleton<PayPalHttpClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            var env = new SandboxEnvironment(
                config["Paypal:ClientId"],
                config["Paypal:Secret"]
            );

            return new PayPalHttpClient(env);
        });
        //services.Configure<TranslationApiSettings>(
        //            configuration.GetSection("TranslationApi")

        //            );

        //services.AddHttpClient<TranslationService>((serviceProvider, client) =>
        //{
        //    var settings = serviceProvider
        //        .GetRequiredService<IOptions<TranslationApiSettings>>()
        //        .Value;

        //    client.BaseAddress = new Uri(settings.BaseUrl);
        //    client.DefaultRequestHeaders.Add("x-api-key", settings.ApiKey);
        //});

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
        services.AddValidatorsFromAssemblyContaining<GetLanuageQuery>();
        services.AddValidatorsFromAssemblyContaining<GetTopDestinationsQuery>();
        services.AddValidatorsFromAssemblyContaining<GetUserToursQuery>();
        
        services.AddValidatorsFromAssemblyContaining<CreateUserTourReviewRequestDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<CheckoutQuoteRequestDtoValidator>();

        services.AddValidatorsFromAssemblyContaining<CreateTourRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<ImageUrlsRequestDTO>();

        services.AddValidatorsFromAssemblyContaining<GetAllCountryInfoQuery>();

        services.AddValidatorsFromAssemblyContaining<GetAllCurrencyQuery>();


        services.AddValidatorsFromAssemblyContaining<CreatePriceRequestDTO>();


        services.AddValidatorsFromAssemblyContaining<CreateTourScheduleRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<CreateAvailableSlotsRequestDTO>();
        services.AddValidatorsFromAssemblyContaining<CreateCancellationRequestDTO>();

        services.AddHostedService<BookingBackgroundService>();
        services.AddHostedService<OutboxWorker>();

        services.AddSingleton<EncryptionService>();

        services.AddScoped<IVoucherService, VoucherService>();

        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IAdminOrderService, AdminOrderService>();

        return services;
    }
}
