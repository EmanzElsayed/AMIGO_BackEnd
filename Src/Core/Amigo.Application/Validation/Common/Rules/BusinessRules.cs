using Amigo.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Validation.Common.Rules
{
    public static class BusinessRules
    {
        public static bool BeAValidCountry(string countryCode)
        {
            return Enum.TryParse(typeof(CountryCode), countryCode, true, out _);
        }

        public static bool BeAValidLanguage(string languageCode)
        {
            return Enum.TryParse(typeof(Language), languageCode, true, out _);
        }
    }
}
