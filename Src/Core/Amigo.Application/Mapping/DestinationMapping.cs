using Amigo.Application.Abstraction.MappingInterfaces;
using Amigo.Domain.DTO.Destination;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.SharedKernal.DTOs.Destination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public class DestinationMapping : IDestinationMapping
    {
        public Destination DestinationToEntity(CreateDestinationRequestDTO requestDTO)
        {
            return new Destination()
            {
                Id = Guid.NewGuid(),
                ImageUrl = requestDTO.ImageUrl,
                ImagePublicId = requestDTO.PublicId,
                IsActive = requestDTO.IsActive,
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

        public IEnumerable< GetTranslationDestinationResponseDTO> EntityToDestination(IEnumerable< DestinationTranslation> destinations)
        {
            var mappedDestination = new List<GetTranslationDestinationResponseDTO>();
            foreach (var destination in destinations)
            { 
                 mappedDestination.Add( new GetTranslationDestinationResponseDTO(
                    TranslationId: destination.Id,
                    Name: destination.Name,
                    Language: destination.Language.ToString(),
                    DestinationDTO:
                    new GetDestinationResponseDTO(destination.DestinationId, destination.Destination.CountryCode.ToString(), destination.Destination.IsActive, destination.Destination.ImageUrl)

                ));
            }
            return mappedDestination;


        }
    }
}
