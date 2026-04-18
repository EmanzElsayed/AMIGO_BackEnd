using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities.TranslationEntities;
[Table($"{nameof(InclusionTranslation)}", Schema = SchemaConstants.translation_schema)]

public class InclusionTranslation : BaseEntity<Guid>
{
    [Required]
    public TourInclusion TourInclusion { get; set; } = null!;
    public Guid TourInclusionId { get; set; }

    [Required]
    public string Text { get; set; } = null!;

    public Language Language { get; set; }

}
