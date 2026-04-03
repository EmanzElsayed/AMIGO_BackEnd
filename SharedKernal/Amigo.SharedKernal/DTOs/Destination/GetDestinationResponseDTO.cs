using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Destination
{
    public record GetDestinationResponseDTO
    (
        Guid DestinationId,
        string CountryCode,
        bool IsActive,
        string? ImageUrl,
        IEnumerable<GetTranslationDestinationResponseDTO> DestinationTranslation 
        
    );
}
