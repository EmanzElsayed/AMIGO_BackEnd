using Amigo.Domain.Enum;
using PhoneNumbers;
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

        public static bool BeAValidGender(string gender)
        {
            return Enum.TryParse(typeof(Gender), gender, true, out _);
        }
        public static bool BeAValidOrderStatus(string orderStatus)
        {
            return Enum.TryParse(typeof(OrderStatus), orderStatus, true, out _);
        }

        public static bool BeAValidCancellation(string cancellationCode)
        {
            return Enum.TryParse(typeof(CancelationPolicyType), cancellationCode, true, out _);
        }

        public static bool BeAValidDateStatus(string availableDateStatus)
        {
            return Enum.TryParse(typeof(AvailableDateTimeStatus), availableDateStatus, true, out _);
        }
        public static bool BeAValidCurrency(string currencyCode)
        {
            return Enum.TryParse(typeof(CurrencyCode), currencyCode, true, out _);

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

        public static bool BeAValidGoogleMapsUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                   (uri.Host.Contains("google.com") || uri.Host.Contains("goo.gl"));
        }




        public static bool IsValidFlagsEnum<T>(T value) where T : Enum
        {
            var allValues = Enum.GetValues(typeof(T))
                                .Cast<int>()
                                .Aggregate((a, b) => a | b);

            return ((int)(object)value & ~allValues) == 0;
        }

        public static bool IsValidFlagsEnumNullable<T>(T? value) where T :struct,  Enum
        {
            if (!value.HasValue)
                return true;

            var allValues = Enum.GetValues(typeof(T))
                                .Cast<int>()
                                .Aggregate((a, b) => a | b);

            return ((int)(object)value & ~allValues) == 0;
        }


        
    }
}
