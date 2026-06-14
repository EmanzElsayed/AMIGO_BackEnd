using Stripe.Tax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Helpers
{
    public static class Constants
    {
        public const CurrencyCode BaseCurrency =
            CurrencyCode.USD;

        public const SupportedLanguage BaseLanguage =
            SupportedLanguage.en;


        public const decimal AverageReviewRating = 9.5m;

        public const int ReviewCount = 1000;

        public const int TravelersCount = 5000;


    }
}
