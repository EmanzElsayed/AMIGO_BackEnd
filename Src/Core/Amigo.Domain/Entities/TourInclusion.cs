using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities
{
    public class TourInclusion: BaseEntity<Guid>
    {
        public Guid TourId { get; set; }
        [Required]
        public Tour Tour { get; set; } = null!;

        public bool IsIncluded { get; set; } = false;

        public ICollection<InclusionTranslation> Translations { get; set; } = new List<InclusionTranslation>();
    }
}
