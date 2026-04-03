using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Destination
{
   public record GetTranslationDestinationResponseDTO
    (
        Guid TranslationId,
        string Name,
        string Language
     );
}
