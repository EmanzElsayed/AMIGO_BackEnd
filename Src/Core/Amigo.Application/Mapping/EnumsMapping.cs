using Amigo.Domain.Enum;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public static class EnumsMapping
    {
        public static CountryCode ToCountryCodeEnum(string country)
        {
            Enum.TryParse<CountryCode>(country, true, out var code);
            return code;
        }

        public static Language ToLanguageEnum(string language)
        {
            Enum.TryParse<Language>(language, true, out var lang);
            return lang;
        }
    }
}
