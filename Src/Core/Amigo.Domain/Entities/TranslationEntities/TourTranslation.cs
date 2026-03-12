using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities.TranslationEntities
{
    public class TourTranslation:BaseEntity<Guid>
    {
        public Guid TourId { get; set; }
        [Required]
        public Tour Tour { get; set; } = null!;

        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public Language Language { get; set; }

        //Guide Language how to translat it and it is enum

    }
}
