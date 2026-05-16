using Amigo.Application.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly HashSet<string> AllowedLanguages =
        [
            "en", "es", "fr", "it", "pt", "br"
        ];
        private static readonly HashSet<string> AllowedCurrencies =
        [
           "BRL", "PEN", "ARS", "COP", "MXN", "USD","EUR","CLP","GBP"
        ];
        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId =>
            _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

        public SupportedLanguage Language
        {
            get
            {
                var languageHeader = _httpContextAccessor.HttpContext?
                    .Request?
                    .Headers["Accept-Language"]
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(languageHeader))
                    return Constants.BaseLanguage;


                // "en-US,en;q=0.9" => "en"
                var lang = languageHeader
                    .Split(',')[0]
                    .Split('-')[0]
                    .Trim()
                    .ToLower();

                lang = AllowedLanguages.Contains(lang)
                    ? lang
                    : Constants.BaseLanguage.ToString();
                return Enum.TryParse<SupportedLanguage>(lang, true, out var parsed)
                    ? parsed
                    : Constants.BaseLanguage;
            }

        }

        public CurrencyCode Currency
        {
            get
            {
                var currencyHeader = _httpContextAccessor.HttpContext?
                    .Request?
                    .Headers["Accept-Currency"]
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(currencyHeader))
                    return Constants.BaseCurrency;


                var currency = currencyHeader
                    .Split(',')[0]
                    .Split('-')[0]
                    .Trim();
                    

                currency = AllowedCurrencies.Contains(currency)
                    ? currency
                    : Constants.BaseCurrency.ToString();
                return Enum.TryParse<CurrencyCode>(currency, true, out var parsed)
                    ? parsed
                    : Constants.BaseCurrency;
            }


        }
    }
}
