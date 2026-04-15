using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities.TranslationEntities
{
    public class CurrencyTranslation:BaseEntity<Guid>
    {
        [Required]
        public Currency Currency { get; set; } = null!;
        public Guid CurrencyId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public Language Language { get; set; }
    }
}
