using Amigo.Domain.DTO.Destination;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.SharedKernal.DTOs.Destination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.MappingInterfaces
{
    public interface IDestinationMapping
    {
        Destination DestinationToEntity(CreateDestinationRequestDTO requestDTO);
        DestinationTranslation DestinationTranslationToEntity(CreateDestinationRequestDTO requestDTO , Destination destination);

        IEnumerable< GetTranslationDestinationResponseDTO> EntityToDestination(IEnumerable< DestinationTranslation> destination);
    }
}
