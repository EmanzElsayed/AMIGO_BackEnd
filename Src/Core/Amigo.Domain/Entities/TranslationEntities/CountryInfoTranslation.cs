using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities.TranslationEntities;

[Table($"{nameof(CountryInfoTranslation)}", Schema = SchemaConstants.translation_schema)]

public class CountryInfoTranslation:BaseEntity<Guid>

{
    [Required]
    public CountryInfo CountryInfo { get; set; } = null!;
    public Guid CountryInfoId { get; set; }

    [Required]
    public string Name { get; set; } = null!;
    public string? Capital {  get; set; }
    public string? OfficialLanguage { get; set; }

    public string? Description { get; set; }
    public SupportedLanguage Language { get; set; }
}
