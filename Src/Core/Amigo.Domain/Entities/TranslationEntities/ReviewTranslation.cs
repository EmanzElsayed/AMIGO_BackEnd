using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities.TranslationEntities
{
    public class ReviewTranslation:BaseEntity<Guid>
    {
        public Guid ReviewId { get; set; }
        [Required]
        public Review Review { get; set; } = null!;

        [Required]
        public string Comment { get; set; } = null!;

        public Language Language {  get; set; }
    }
}
