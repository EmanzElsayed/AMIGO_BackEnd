using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Translation
{
    public class DestinationTranslationItem
    {
        public SupportedLanguage SourceLanguage { get; set; }

        public Guid DestinationId { get; set; }
        public string Name { get; set; } = null!;

    }
}
