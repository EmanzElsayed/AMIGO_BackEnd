using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Translation
{
    public class TourTranslationAiResult
    {
        public string Language { get; set; } = null!;

        public List<TourTranslationItem> Tours { get; set; }
            = new();
    }
}
