using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities.TranslationEntities;
[Table($"{nameof(PriceTranslation)}", Schema = SchemaConstants.tour_schema)]


public class PriceTranslation : BaseEntity<Guid>
{
                                               
    public Price Price { get; set; } = null!;
    public Guid PriceId { get; set; }
    
    [Required]
    public string Type { get; set; } = null!;

    public Language Language { get; set; }
    
}

