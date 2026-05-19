using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Translation
{
    public class TourTranslationItem
    {
        public Guid TourId { get; set; }
        public SupportedLanguage SourceLanguage { get; set; }

        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public CancellationTranslationItem? Cancellation { get; set; }
        public DestinationTranslationItem? Destination{ get; set; }

        public List<InclusionTranslationItem> Inclusions { get; set; } = new();

        public List<PriceTranslationItem> Prices { get; set; } = new();
    }
}
