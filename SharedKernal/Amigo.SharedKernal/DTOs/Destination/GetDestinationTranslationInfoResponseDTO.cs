using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Destination
{
    public record GetDestinationTranslationInfoResponseDTO
    (
        Guid DestinaionId, 
        Guid TranslationId,
         string Name,
         string Language
     );
}
