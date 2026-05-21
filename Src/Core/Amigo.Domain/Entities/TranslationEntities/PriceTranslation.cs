using Amigo.Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities.TranslationEntities;
[Table($"{nameof(PriceTranslation)}", Schema = SchemaConstants.translation_schema)]


    public class PriceTranslation : BaseEntity<Guid>
    {
                                               
        public Price Price { get; set; } = null!;
        public Guid PriceId { get; set; }
    
        [Required]
        public string Type { get; set; } = null!; // adult or child

       
        public string? ActivityType { get; set; } // standard or luxury
        
        public SupportedLanguage Language { get; set; }

    }

