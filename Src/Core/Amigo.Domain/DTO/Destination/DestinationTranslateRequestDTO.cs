using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Destination
{
    public class DestinationTranslateRequestDTO
    {
        public Guid DestinationId { get; set; }
        public SupportedLanguage SourceLanguage { get; set; }
    }
}
