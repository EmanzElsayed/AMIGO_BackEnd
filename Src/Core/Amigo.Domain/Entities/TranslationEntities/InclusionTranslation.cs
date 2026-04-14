using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities.TranslationEntities
{
    public class InclusionTranslation : BaseEntity<Guid>
    {
        [Required]
        public TourInclusion TourInclusion { get; set; } = null!;
        public Guid TourInclusionId { get; set; }

        [Required]
        public string Text { get; set; } = null!;

        public Language Language { get; set; }

    }
}
