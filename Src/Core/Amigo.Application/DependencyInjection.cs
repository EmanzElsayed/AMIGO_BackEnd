using Amigo.Application.Abstraction;
using Amigo.Application.BackgroundTasks;
using Amigo.Application.Abstraction.Services;
using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Application.Abstraction.Services.Authentication;
using Amigo.Application.Mapping;
using Amigo.Application.Services;
using Amigo.Application.Services.Admin;
using Amigo.Application.Services.AutoTranslation;
using Amigo.Application.Services.BackGroundServices;
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



        //Services
        services.AddScoped<IServiceManager, ServiceManagerWithFactoryDelegate>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<Func<IAuthService>>(provider => () => provider.GetRequiredService<IAuthService>());


        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<Func<IEmailService>>(provider => () => provider.GetRequiredService<IEmailService>());

        services.AddScoped<IEnumService, EnumService>();
        services.AddScoped<Func<IEnumService>>(provider => () => provider.GetRequiredService<IEnumService>());


        services.AddScoped<IDestinationService , DestinationService>();
        services.AddScoped<Func<IDestinationService>>(provider => () => provider.GetRequiredService<IDestinationService>());

        services.AddScoped<IUserTourCatalogService, UserTourCatalogService>();
        services.AddScoped<Func<IUserTourCatalogService>>(provider => () => provider.GetRequiredService<IUserTourCatalogService>());

        services.AddScoped<IUserTourReviewService, UserTourReviewService>();
        services.AddScoped<Func<IUserTourReviewService>>(provider => () => provider.GetRequiredService<IUserTourReviewService>());



        services.AddScoped<IAdminDestinationService, AdminDestinationService>();
        services.AddScoped<Func<IAdminDestinationService>>(provider => () => provider.GetRequiredService<IAdminDestinationService>());


        services.AddScoped<IAdminTourService, AdminTourService>();
        services.AddScoped<Func<IAdminTourService>>(provider => () => provider.GetRequiredService<IAdminTourService>());


        services.AddScoped<IAdminPriceService, AdminPriceService>();
        services.AddScoped<Func<IAdminPriceService>>(provider => () => provider.GetRequiredService<IAdminPriceService>());


        services.AddScoped<IAdminAvailableSlotsService, AdminAvailableSlotsService>();
        services.AddScoped<Func<IAdminAvailableSlotsService>>(provider => () => provider.GetRequiredService<IAdminAvailableSlotsService>());

        services.AddScoped<IAdminTourScheduleService, AdminTourScheduleService>();
        services.AddScoped<Func<IAdminTourScheduleService>>(provider => () => provider.GetRequiredService<IAdminTourScheduleService>());

        services.AddScoped<IAdminTourInclusionService, AdminTourInclusionService>();
        services.AddScoped<Func<IAdminTourInclusionService>>(provider => () => provider.GetRequiredService<IAdminTourInclusionService>());

        services.AddScoped<IAdminTourCancellationService, AdminTourCancellationService>();
        services.AddScoped<Func<IAdminTourCancellationService>>(provider => () => provider.GetRequiredService<IAdminTourCancellationService>());

        services.AddScoped<IAdminCustomerService, AdminCustomerService>();
        services.AddScoped<Func<IAdminCustomerService>>(provider => () => provider.GetRequiredService<IAdminCustomerService>());


        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<Func<ICurrencyService>>(provider => () => provider.GetRequiredService<ICurrencyService>());


        services.AddScoped<IUserService, UserService>();
        services.AddScoped<Func<IUserService>>(provider => () => provider.GetRequiredService<IUserService>());

        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<Func<IFavoriteService>>(provider => () => provider.GetRequiredService<IFavoriteService>());


        services.AddScoped<ICountryInfoService, CountryInfoService>();
        services.AddScoped<Func<ICountryInfoService>>(provider => () => provider.GetRequiredService<ICountryInfoService>());


        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<Func<IImageService>>(provider => () => provider.GetRequiredService<IImageService>());


        services.AddScoped<IPhoneCodeService, PhoneCodeService>();
        services.AddScoped<Func<IPhoneCodeService>>(provider => () => provider.GetRequiredService<IPhoneCodeService>());


        services.AddScoped<ICartService, CartService>();
        services.AddScoped<Func<ICartService>>(provider => () => provider.GetRequiredService<ICartService>());


        services.AddScoped<IPaymentOrchestrator, PaymentOrchestrator>();
        services.AddScoped<Func<IPaymentOrchestrator>>(provider => () => provider.GetRequiredService<IPaymentOrchestrator>());


        services.AddScoped<IJWTTokenService, JWTTokenService>();
        services.AddScoped<Func<IJWTTokenService>>(provider => () => provider.GetRequiredService<IJWTTokenService>());


        services.AddScoped<ITravelersService, TravelersService>();
        services.AddScoped<Func<ITravelersService>>(provider => () => provider.GetRequiredService<ITravelersService>());


        services.AddScoped<IPaymentProviderResolver, PaymentProviderResolver>();
        services.AddScoped<Func<IPaymentProviderResolver>>(provider => () => provider.GetRequiredService<IPaymentProviderResolver>());

        services.AddScoped<IPaymentProvider, StripePaymentProvider>();
        services.AddScoped<IPaymentProvider, PaypalPaymentProvider>();
        services.AddScoped<Func<IPaymentProvider>>(provider => () => provider.GetRequiredService<IPaymentProvider>());



        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<Func<IPaymentService>>(provider => () => provider.GetRequiredService<IPaymentService>());

        services.AddScoped<ICheckoutQuoteService, CheckoutQuoteService>();
        services.AddScoped<Func<ICheckoutQuoteService>>(provider => () => provider.GetRequiredService<ICheckoutQuoteService>());

        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<Func<IBookingService>>(provider => () => provider.GetRequiredService<IBookingService>());



        services.AddScoped<IVoucherService, VoucherService>();
        services.AddScoped<Func<IVoucherService>>(provider => () => provider.GetRequiredService<IVoucherService>());

        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<Func<IOrderService>>(provider => () => provider.GetRequiredService<IOrderService>());

        services.AddScoped<IAdminOrderService, AdminOrderService>();
        services.AddScoped<Func<IAdminOrderService>>(provider => () => provider.GetRequiredService<IAdminOrderService>());

        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<Func<ICacheService>>(provider => () => provider.GetRequiredService<ICacheService>());


        services.AddScoped<ICurrencyRateService, CurrencyRateService>();
        services.AddScoped<Func<ICurrencyRateService>>(provider => () => provider.GetRequiredService<ICurrencyRateService>());

        services.AddScoped<ICurrencyProvider, CurrencyApiProvider>();
        services.AddScoped<Func<ICurrencyProvider>>(provider => () => provider.GetRequiredService<ICurrencyProvider>());

        //services.AddScoped<IOpenAiBatchTranslationService, OpenAiBatchTranslationService>();
        //services.AddScoped<Func<IOpenAiBatchTranslationService>>(provider => () => provider.GetRequiredService<IOpenAiBatchTranslationService>());

        services.AddScoped<IOpenAiBatchTranslationService, GeminiBatchTranslationService>();
        services.AddScoped<Func<IOpenAiBatchTranslationService>>(provider => () => provider.GetRequiredService<IOpenAiBatchTranslationService>());

        services.AddScoped<IAutoTranslationService, TourBatchTranslationService>();
        services.AddScoped<Func<IAutoTranslationService>>(provider => () => provider.GetRequiredService<IAutoTranslationService>());

        services.AddScoped<IAdminCancellationService, AdminCancellationService>();
        services.AddScoped<Func<IAdminCancellationService>>(provider => () => provider.GetRequiredService<IAdminCancellationService>());


        services.AddScoped<
          ICurrentUserService,
          CurrentUserService>();
        services.AddScoped<Func<ICurrentUserService>>(provider => () => provider.GetRequiredService<ICurrentUserService>());

        services
     .AddScoped<IRefundProvider,
     PaypalRefundProvider>();
        services.AddScoped<Func<IRefundProvider>>(provider => () => provider.GetRequiredService<IRefundProvider>());

        services
            .AddScoped<IRefundProviderResolver,
                RefundProviderResolver>();
        services.AddScoped<Func<IRefundProviderResolver>>(provider => () => provider.GetRequiredService<IRefundProviderResolver>());




        services.AddHttpClient<IPaymentProvider, PayTabsPaymentProvider>(client =>
        {
            client.BaseAddress = new Uri("https://secure-egypt.paytabs.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddSingleton<PayPalHttpClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            var env = new SandboxEnvironment(
                config["Paypal:ClientId"],
                config["Paypal:Secret"]
            );

            return new PayPalHttpClient(env);
        });
      

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

        services.AddValidatorsFromAssemblyContaining<PiceWithActivityTypeRequestQuery>();


        services.AddHostedService<BookingBackgroundService>();
        services.AddHostedService<OutboxWorker>();

        services.AddHostedService<QueueBackgroundService>();

        services.AddSingleton<EncryptionService>();


        services.AddHttpClient();

      
        services.AddHttpClient<ITranslationService, GeminiTranslationService>();

        
        services.AddHttpContextAccessor();

      

        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();


     

        services
            .AddHostedService<
                RefundProcessorService>();
        return services;
    }
}
