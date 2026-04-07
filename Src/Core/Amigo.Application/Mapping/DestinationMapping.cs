using Amigo.Application.Abstraction.MappingInterfaces;
using Amigo.Application.Services;
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
    public class DestinationMapping(ImageCloudService _imageCloud) : IDestinationMapping
    {
        public Destination DestinationToEntity(CreateDestinationRequestDTO requestDTO)
        {
            return new Destination()
            {
                Id = Guid.NewGuid(),
                ImageUrl = requestDTO.ImageUrl,
                ImagePublicId = requestDTO.PublicId,
                IsActive = requestDTO.IsActive ?? true,
                CountryCode = EnumsMapping.ToCountryCodeEnum(requestDTO.CountryCode),
            };
        }

        public DestinationTranslation DestinationTranslationToEntity(CreateDestinationRequestDTO requestDTO , Destination dest)
        {
            return new DestinationTranslation()
            {
                Id = Guid.NewGuid(),
                Name = requestDTO.Name,
                Language = EnumsMapping.ToLanguageEnum(requestDTO.Language),
                Destination = dest,
                DestinationId = dest.Id
            };
        }

        public IEnumerable<GetDestinationResponseDTO> EntitiesToDestinations(IEnumerable<Destination> destinations)
        {
             int numberOfLanguage = Enum.GetValues<Language>().Length;

            return destinations.Select(destination => new GetDestinationResponseDTO(

                DestinationId: destination.Id,
                CountryCode: destination.CountryCode.ToString(),
                IsActive: destination.IsActive,
                ImageUrl: destination.ImageUrl,
                IsFullyTranslated : (destination.Translations.Count == numberOfLanguage ?true : false ) ,

                DestinationTranslation: destination.Translations.Select(translation =>
                    new GetTranslationDestinationResponseDTO(
                        TranslationId: translation.Id,
                        Name: translation.Name,
                        Language: translation.Language.ToString()
                    )
                    
                ) 
                
            ));
        }
        public GetDestinationResponseDTO EntityToAdminDestination(Destination destination ,  string? language)
        {

            bool isFullyTranslated = destination.Translations.Count == Enum.GetValues<Language>().Length? true:false;  
              return  new GetDestinationResponseDTO(
                   DestinationId: destination.Id,
                   CountryCode: destination.CountryCode.ToString(),
                   IsActive: destination.IsActive,
                    ImageUrl: destination.ImageUrl,
                    IsFullyTranslated: isFullyTranslated,
                DestinationTranslation: destination.Translations.Any() ? destination.Translations.Select(translation =>
                    new GetTranslationDestinationResponseDTO(
                        TranslationId: translation.Id,
                        Name: translation.Name,
                        Language: translation.Language.ToString()
                    )
                 ) : new List<GetTranslationDestinationResponseDTO> {

                    new GetTranslationDestinationResponseDTO(
                            TranslationId: (Guid?)null,
                            Name: "",
                            Language: language
                    )
                 }
              );
           


        }


        public GetDestinationResponseDTO EntityToDestination(Destination destination, string? language)
        {

            bool isFullyTranslated = destination.Translations.Count == Enum.GetValues<Language>().Length ? true : false;
            return new GetDestinationResponseDTO(
                 DestinationId: destination.Id,
                 CountryCode: destination.CountryCode.ToString(),
                 IsActive: destination.IsActive,
                  ImageUrl: destination.ImageUrl,
                  IsFullyTranslated: isFullyTranslated,
              DestinationTranslation:  destination.Translations.Select(translation =>
                  new GetTranslationDestinationResponseDTO(
                      TranslationId: translation.Id,
                      Name: translation.Name,
                      Language: translation.Language.ToString()
                  )
               ) 
            );



        }

        public void UpdateDestination(
             UpdateDestinationRequestDTO requestDTO,
             Destination destination,
             DestinationTranslation? translation,
             Language? language)
        {
            
            if (requestDTO.IsActive is not null)
                destination.IsActive = requestDTO.IsActive.Value;

            if (requestDTO.CountryCode is not null)
                destination.CountryCode = EnumsMapping.ToCountryCodeEnum(requestDTO.CountryCode);

            if (requestDTO.Name is not null && language is not null)
            {
                if (translation is null)
                {
                    // add new language
                    destination.Translations.Add(new DestinationTranslation
                    {
                        Language = language.Value,
                        Name = requestDTO.Name,
                        DestinationId = destination.Id,
                    });
                }
                else
                {
                    //  update existing language
                    translation.Name = requestDTO.Name;
                }
            }

            // image logic
            if (requestDTO.ImageUrl is not null)
            {
                destination.ImageUrl = requestDTO.ImageUrl;

                if (destination.ImagePublicId is not null)
                    _imageCloud.DeleteImage(destination.ImagePublicId);

                if (requestDTO.PublicId is not null)
                    destination.ImagePublicId = requestDTO.PublicId;
            }
        }
    }
}
