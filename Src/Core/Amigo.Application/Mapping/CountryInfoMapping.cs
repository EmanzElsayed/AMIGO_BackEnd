using Amigo.Domain.DTO.CountryInfo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public static class CountryInfoMapping
    {
        public static List<GetCountryInfoResponseDTO> FromEntitiesToDTOs(this IEnumerable<CountryInfo> countriesInfo)
        {
            var result =  countriesInfo.Select(c => new GetCountryInfoResponseDTO(

                CountryInfoId : c.Id,
                CountryCode : c.CountryCode.ToString(),
                PhoneCode : c.PhoneCode,
                c.Translations.FirstOrDefault()?.Name ?? string.Empty,
                c.Translations.FirstOrDefault()?.Language.ToString() ?? string.Empty



                )).ToList();
            return result;
        }
        public static GetCountryInfoResponseDTO FromEntityToDTO(this CountryInfo countryInfo)
        {
            var result = new GetCountryInfoResponseDTO(

               countryInfo.Id,
               countryInfo.CountryCode.ToString(),
               countryInfo.PhoneCode,
               countryInfo.Translations.FirstOrDefault()?.Name ?? string.Empty,
               countryInfo.Translations.FirstOrDefault()?.Language.ToString() ?? string.Empty



                );
            return result;
        }
    }
}
