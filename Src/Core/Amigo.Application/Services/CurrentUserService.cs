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

               
                return Enum.TryParse<SupportedLanguage>(lang, true, out var parsed)
                      && Enum.IsDefined(typeof(SupportedLanguage), parsed)
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


                return Enum.TryParse<CurrencyCode>(currency, true, out var parsed)
                        && Enum.IsDefined(typeof(CurrencyCode), parsed)
                     ? parsed
                     : Constants.BaseCurrency;
            }


        }
    }
}
