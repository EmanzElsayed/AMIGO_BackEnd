using Amigo.Domain.DTO.CountryInfo;
using Amigo.SharedKernal.DTOs.Currency;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public static class CurrencyMapping
    {
        public static List<GetCurrencyResponseDTO> FromEntitiesToDTOs(this IEnumerable<Currency> currencies)
        {
            var result = currencies.Select(c => new GetCurrencyResponseDTO(

                 c.Id,
                 c.CurrencyCode.ToString(),
                 c.Icon,
                c.CodeIcon,
                 c.Translations.FirstOrDefault()?.Name ?? string.Empty,
                c.Translations.FirstOrDefault()?.Language.ToString() ?? string.Empty


                )).ToList();
            return result;
        }
        public static GetCurrencyResponseDTO FromEntityToDTO(this Currency c)
        {
            var result = new GetCurrencyResponseDTO(

                c.Id,
                 c.CurrencyCode.ToString(),
                 c.Icon,
                c.CodeIcon,
                 c.Translations.FirstOrDefault()?.Name ?? string.Empty,
                c.Translations.FirstOrDefault()?.Language.ToString() ?? string.Empty



                );
            return result;
        }
    }
}
