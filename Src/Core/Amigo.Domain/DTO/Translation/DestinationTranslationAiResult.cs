using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Translation
{
    public class DestinationTranslationAiResult
    {
        public string Language { get; set; } = null!;
        public DestinationTranslationItem Destination { get; set; } = null!;
    }
}
