using Amigo.Application.Helpers;
using Amigo.Application.Services;
using Amigo.Domain.DTO.CountryInfo;
using Amigo.Domain.DTO.Destination;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.DTOs.Destination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public static class  DestinationMapping
    {
        public static Destination DestinationToEntity(this CreateDestinationRequestDTO requestDTO,CountryInfo country)
        {

            return new Destination()
            {
                Id = Guid.NewGuid(),
                ImageUrl = requestDTO.ImageUrl,
                ImagePublicId = requestDTO.PublicId,
                IsActive = requestDTO.IsActive ?? true,
                CountryInfoId = country.Id,
                CountryInfo = country,
                Translations = new List<DestinationTranslation>
                {
                    new DestinationTranslation()
                    {
                        Id = Guid.NewGuid(),
                        Name = requestDTO.Name,
                        Language = EnumsMapping.ToLanguageEnum(requestDTO.Language)

                    }
                }


                
            };
        }

        

        public static IEnumerable<GetDestinationResponseDTO> EntitiesToDestinations(IEnumerable<Destination> destinations,SupportedLanguage language)
        {

            return destinations.Select(destination => new GetDestinationResponseDTO(

                DestinationId: destination.Id,
                Country: destination.CountryInfo is  null? null : new GetCountryInfoResponseDTO(
                        destination.CountryInfo.Id ,
                        destination.CountryInfo.CountryCode.ToString(),
                        destination.CountryInfo.PhoneCode,
                        destination.CountryInfo.Translations.Where(c => c.Language == language).Select(c => c.Name).FirstOrDefault(),
                        language.ToString()
                    ),
                
                IsActive: destination.IsActive,
                ImageUrl: destination.ImageUrl,
                Name :  destination.Translations.Where(c => c.Language == language).Select(c => c.Name).FirstOrDefault(),
                Language : language.ToString()
                
            ));
        }
        public static GetDestinationDetailsDTO EntityToAdminDestination(Destination destination ,  SupportedLanguage? language)
        {
            if (language is null) language = Constants.BaseLanguage;

            return new GetDestinationDetailsDTO(
                   DestinationId: destination.Id,
                     Country: destination.CountryInfo is null ? null : new CountryInfoDetailsDTO(
                        destination.CountryInfo.Id,
                        destination.CountryInfo.CountryCode.ToString(),
                        destination.CountryInfo.PhoneCode,
                        destination.CountryInfo.Translations.Where(c => c.Language == language).Select(c => c.Name).FirstOrDefault(),
                        language.ToString(),
                        destination.CountryInfo.ImageUrl,
                        destination.CountryInfo.Translations.Where(c => c.Language == language).Select(c => c.Description).FirstOrDefault()


                    ),
                   IsActive: destination.IsActive,
                   ImageUrl: destination.ImageUrl,
                                       
                   Name: !destination.Translations.Any()? "": destination.Translations.Where(c => c.Language == language).Select(c => c.Name).FirstOrDefault() ?? "",
                   Language : language.ToString(),
                   Description : !destination.Translations.Any() ? "" : destination.Translations.Where(c => c.Language == language).Select(c => c.Description).FirstOrDefault() ?? ""


              );
           


        }


        public static GetDestinationByIdResponseDTO EntityToDestination(Destination destination, SupportedLanguage language,decimal averageRating, int travelersCount,int reviewsCount,int toursCount)
        {


            return new GetDestinationByIdResponseDTO(
                 DestinationId: destination.Id,
                  Country: destination.CountryInfo is null ? null: new GetCountryInfoResponseDTO(
                        destination.CountryInfo.Id,
                        destination.CountryInfo.CountryCode.ToString(),
                        destination.CountryInfo.PhoneCode,
                        destination.CountryInfo.Translations.Where(c => c.Language == language).Select(c => c.Name).FirstOrDefault(),
                        language.ToString()
                    ),
                  IsActive: destination.IsActive,
                  ImageUrl: destination.ImageUrl,

                  Name: destination.Translations.Where(c => c.Language == language).Select(c => c.Name).FirstOrDefault(),
                  Language: language.ToString(),
                  ReviewsCount : reviewsCount,
                  TravelersCount: travelersCount,
                  AverageReviewRating : averageRating,
                  ToursCount: toursCount,
                  Description: destination.Translations.Where(c => c.Language == language).Select(c => c.Description).FirstOrDefault()
            );



        }

        public static void UpdateDestination(
             UpdateDestinationRequestDTO requestDTO,
             Destination destination,
             DestinationTranslation? translation,
             SupportedLanguage? language)
        {
            
            if (requestDTO.IsActive is not null)
                destination.IsActive = requestDTO.IsActive.Value;

         
            if ((requestDTO.Name is not null || requestDTO.Description is not null) && language is not null)
            {
                if (translation is null)
                {
                    // add new language
                    destination.Translations.Add(new DestinationTranslation
                    {
                        Language = language.Value,
                        Name = requestDTO.Name ?? "",
                        Description = requestDTO.Description ?? "",
                        DestinationId = destination.Id,
                    });
                }
                else
                {
                    //  update existing language
                    translation.Name = requestDTO.Name ?? translation.Name;
                    translation.Description = requestDTO.Description ?? translation.Description;
                }
            }

        }
    }
}
