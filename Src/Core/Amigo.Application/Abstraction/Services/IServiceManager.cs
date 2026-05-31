using Amigo.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IServiceManager
    {
        IAuthService AuthService { get; }
        IJWTTokenService JWTTokenService { get; }
        IEmailService EmailService { get; }
        IAutoTranslationService AutoTranslationService { get; }
        IBookingService BookingService { get; }
        ICartService CartService { get; }
        ICheckoutQuoteService CheckoutQuoteService { get; }

        ICountryInfoService CountryInfoService { get; }

        ICurrencyProvider CurrencyProvider { get; }

        ICurrencyRateService CurrencyRateService { get; }

        ICurrencyService CurrencyService { get; }

        ICurrentUserService CurrentUserService { get; }

        IDestinationService DestinationService { get; }

        IDestinationSlugResolver DestinationSlugResolver { get; }

        IEnumService EnumService { get; }

        IFavoriteService FavoriteService { get; }
        IImageService ImageService { get; }

        ILocalizationService LocalizationService { get; }

        IOpenAiBatchTranslationService OpenAiBatchTranslationService { get; }

        IOrderService OrderService { get; }

        IPaymentOrchestrator PaymentOrchestrator { get; }

        IPaymentProvider PaymentProvider { get; }

        IPaymentProviderResolver PaymentProviderResolver { get; }

        IPaymentService PaymentService { get; }

        IPhoneCodeService PhoneCodeService { get; }

        IRefundProvider RefundProvider { get; }

        IRefundProviderResolver RefundProviderResolver { get; }

        ITopDestinationsReader TopDestinationsReader { get; }

        ITourReviewEligibilityReader TourReviewEligibilityReader { get; }

        ITourTranslationQueryService TourTranslationQueryService { get; }

      

        ITravelersService TravelersService { get; }

        IUserService UserService { get; }

        IUserTourCatalogService UserTourCatalogService { get; }

        IUserTourReviewService UserTourReviewService { get; }
        IVoucherService VoucherService { get; }

        IAdminDestinationService AdminDestinationService { get; }
        IAdminTourService AdminTourService { get; }

        IAdminPriceService AdminPriceService { get; }

        IAdminAvailableSlotsService AdminAvailableSlotsService { get; }

        IAdminTourScheduleService AdminTourScheduleService { get; }

        IAdminTourInclusionService AdminTourInclusionService { get; }

        IAdminTourCancellationService AdminTourCancellationService { get; }
        IAdminCancellationService AdminCancellationService { get; }
        IAdminCustomerService AdminCustomerService { get; }
        IAdminOrderService AdminOrderService { get; }
        ICacheService CacheService { get; }
    }
}
