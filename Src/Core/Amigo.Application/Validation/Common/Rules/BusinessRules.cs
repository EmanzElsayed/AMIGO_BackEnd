using Amigo.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
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
            //return Enum.TryParse(typeof(Language), languageCode, true, out _);
            if (string.IsNullOrWhiteSpace(languageCode))
                return false;

            foreach (var field in typeof(Language).GetFields())
            {
                // check enum name
                if (field.Name.Equals(languageCode, StringComparison.OrdinalIgnoreCase))
                    return true;

                // check display name
                var attribute = field.GetCustomAttribute<DisplayAttribute>();
                if (attribute != null && attribute.Name.Equals(languageCode, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;

        }

        public static bool TryCleanGuid(string input, out Guid guid)
        {
            guid = Guid.Empty;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            string candidate = input.Trim().Trim('"'); 

            if (candidate.Length > 36)
                candidate = candidate.Substring(0, 36);

            return Guid.TryParse(candidate, out guid);
        }
    }
}
