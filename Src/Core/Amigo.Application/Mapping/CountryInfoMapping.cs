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

        public static GetCountryByIdResponseDTO EntityToCountry(CountryInfo country, SupportedLanguage language, decimal averageRating, int travelersCount, int reviewsCount, int toursCount,int destinationCount)
        {


            return new GetCountryByIdResponseDTO(
                 CountryId: country.Id,
                  CountryCode : country.CountryCode.ToString(),
                  PhoneCode: country.PhoneCode,
                  Capital : country.Translations.Where(c => c.Language == language).Select(c => c.Capital).FirstOrDefault(),
                  OfficialLanguage: country.Translations.Where(c => c.Language == language).Select(c => c.OfficialLanguage).FirstOrDefault(),
                  ImageUrl: country.ImageUrl,

                  Name: country.Translations.Where(c => c.Language == language).Select(c => c.Name).FirstOrDefault(),
                  Language: language.ToString(),
                  ReviewsCount: reviewsCount,
                  TravelersCount: travelersCount,
                  AverageReviewRating: averageRating,
                  ToursCount: toursCount,
                  DestinationCount : destinationCount,
                  Description: country.Translations.Where(c => c.Language == language).Select(c => c.Description).FirstOrDefault()
            );



        }
    }
}
