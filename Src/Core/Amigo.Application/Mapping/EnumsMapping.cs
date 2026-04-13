using Amigo.Domain.Enum;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
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
            if (string.IsNullOrWhiteSpace(language))
                return Language.English;

            if (Enum.TryParse<Language>(language, true, out var parsed) && parsed != Language.None)
                return parsed;

            foreach (var field in typeof(Language).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.Name.Equals(language, StringComparison.OrdinalIgnoreCase))
                    return (Language)field.GetValue(null)!;

                var display = field.GetCustomAttribute<DisplayAttribute>();
                if (display?.Name != null && display.Name.Equals(language, StringComparison.OrdinalIgnoreCase))
                    return (Language)field.GetValue(null)!;
            }

            return Language.English;
        }
        public static AvailableDateTimeStatus ToAvailableSheduleStatus(string? availableStatus)
        {
            if (availableStatus is null) return AvailableDateTimeStatus.Available;
            Enum.TryParse<AvailableDateTimeStatus>(availableStatus, true, out var status);
            return status;
        }
       
        public static T ToEnum<T>(string? value, bool acceptNull) where T : struct, Enum
        {
            // helper to get enum value by underlying int
            static T FromInt(int number) =>
                (T)Enum.ToObject(typeof(T), number);

            if (string.IsNullOrWhiteSpace(value))
            {
                // return 0 when acceptNull = true, otherwise 1
                return FromInt(acceptNull ? 0 : 1);
            }

            if (Enum.TryParse<T>(value, true, out var result))
            {
                return result;
            }

            // fallback: return 1 if parsing fails
            return FromInt(1);
        }
    }
}
