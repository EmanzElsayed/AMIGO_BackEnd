using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class ServiceManagerWithFactoryDelegate(
        Func<IAutoTranslationService> autoTranslationService,
        Func<IBookingService> bookingService,
        Func<ICartService> cartService,
        Func<ICheckoutQuoteService> checkoutQuoteService,
        Func<ICountryInfoService> countryInfoService,
        Func<ICurrencyProvider> currencyProvider,
        Func<ICurrencyRateService> currencyRateService,
        Func<ICurrencyService> currencyService,
        Func<ICurrentUserService> currentUserService,
        Func<IDestinationService> destinationService,
        Func<IDestinationSlugResolver> destinationSlugResolver,
        Func<IEnumService> enumService,
        Func<IFavoriteService> favoriteService,
        Func<IImageService> imageService,
        Func<ILocalizationService> localizationService,
        Func<IOpenAiBatchTranslationService> openAiBatchTranslationService,
        Func<IOrderService> orderService,
        Func<IPaymentOrchestrator> paymentOrchestrator,
        Func<IPaymentProvider> paymentProvider,
        Func<IPaymentProviderResolver> paymentProviderResolver,
        Func<IPaymentService> paymentService,
        Func<IPhoneCodeService> phoneCodeService,
        Func<IRefundProvider> refundProvider,
        Func<IRefundProviderResolver> refundProviderResolver,
        Func<ITopDestinationsReader> topDestinationsReader,
        Func<ITourReviewEligibilityReader> tourReviewEligibilityReader,
        Func<ITourTranslationQueryService> tourTranslationQueryService,
     
        Func<ITravelersService> travelersService,
        Func<IUserService> userService,
        Func<IUserTourCatalogService> userTourCatalogService,
        Func<IUserTourReviewService> userTourReviewService,
        Func<IVoucherService> voucherService,
        Func<IEmailService> emailService,
        Func<IAdminDestinationService> adminDestinationService,
        Func<IAdminTourService> adminTourService,
        Func<IAdminPriceService> adminPriceService,
        Func<IAdminAvailableSlotsService> adminAvailableSlotsService,
        Func<IAdminTourScheduleService> adminTourScheduleService,
        Func<IAdminTourInclusionService> adminTourInclusionService,
        Func<IAdminTourCancellationService> adminTourCancellationService,
        Func<IAdminCustomerService> adminCustomerService,
        Func<IJWTTokenService> jWTTokenService,
        Func<IAdminOrderService> adminOrderService,
        Func<ICacheService> cacheService,
        Func<IAdminCancellationService> adminCancellationService,
        Func<IAuthService> authService

        ) : IServiceManager
    {
        public IAutoTranslationService AutoTranslationService => autoTranslationService.Invoke();

        public IBookingService BookingService => bookingService.Invoke();

        public ICartService CartService => cartService.Invoke();

        public ICheckoutQuoteService CheckoutQuoteService => checkoutQuoteService.Invoke();

        public ICountryInfoService CountryInfoService => countryInfoService.Invoke();

        public ICurrencyProvider CurrencyProvider => currencyProvider.Invoke();

        public ICurrencyRateService CurrencyRateService => currencyRateService.Invoke();

        public ICurrencyService CurrencyService => currencyService.Invoke();

        public ICurrentUserService CurrentUserService => currentUserService.Invoke();

        public IDestinationService DestinationService => destinationService.Invoke();

        public IDestinationSlugResolver DestinationSlugResolver => destinationSlugResolver.Invoke();

        public IEnumService EnumService => enumService.Invoke();

        public IFavoriteService FavoriteService => favoriteService.Invoke();

        public IImageService ImageService => imageService.Invoke();

        public ILocalizationService LocalizationService => localizationService.Invoke();

        public IOpenAiBatchTranslationService OpenAiBatchTranslationService => openAiBatchTranslationService.Invoke();

        public IOrderService OrderService => orderService.Invoke();

        public IPaymentOrchestrator PaymentOrchestrator => paymentOrchestrator.Invoke();

        public IPaymentProvider PaymentProvider => paymentProvider.Invoke();

        public IPaymentProviderResolver PaymentProviderResolver => paymentProviderResolver.Invoke();

        public IPaymentService PaymentService => paymentService.Invoke();

        public IPhoneCodeService PhoneCodeService => phoneCodeService.Invoke();

        public IRefundProvider RefundProvider => refundProvider.Invoke();

        public IRefundProviderResolver RefundProviderResolver => refundProviderResolver.Invoke();

        public ITopDestinationsReader TopDestinationsReader => topDestinationsReader.Invoke();

        public ITourReviewEligibilityReader TourReviewEligibilityReader => tourReviewEligibilityReader.Invoke();

        public ITourTranslationQueryService TourTranslationQueryService => tourTranslationQueryService.Invoke();

      
        public ITravelersService TravelersService => travelersService.Invoke();

        public IUserService UserService => userService.Invoke();

        public IUserTourCatalogService UserTourCatalogService => userTourCatalogService.Invoke();

        public IUserTourReviewService UserTourReviewService => userTourReviewService.Invoke();

        public IVoucherService VoucherService => voucherService.Invoke();

        public IEmailService EmailService => emailService.Invoke();

        public IAdminDestinationService AdminDestinationService => adminDestinationService.Invoke();

        public IAdminTourService AdminTourService => adminTourService.Invoke();

        public IAdminPriceService AdminPriceService => adminPriceService.Invoke();

        public IAdminAvailableSlotsService AdminAvailableSlotsService => adminAvailableSlotsService.Invoke();

        public IAdminTourScheduleService AdminTourScheduleService => adminTourScheduleService.Invoke();

        public IAdminTourInclusionService AdminTourInclusionService => adminTourInclusionService.Invoke();

        public IAdminTourCancellationService AdminTourCancellationService => adminTourCancellationService.Invoke();

        public IAdminCustomerService AdminCustomerService => adminCustomerService.Invoke();

        public IJWTTokenService JWTTokenService => jWTTokenService.Invoke();

        public IAdminOrderService AdminOrderService => adminOrderService.Invoke();

        public ICacheService CacheService => cacheService.Invoke();

        public IAdminCancellationService AdminCancellationService => adminCancellationService.Invoke();

        public IAuthService AuthService => authService.Invoke();
    }
}
