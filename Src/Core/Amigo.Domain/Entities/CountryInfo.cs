using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities;

[Table($"{nameof(CountryInfo)}", Schema = SchemaConstants.shared_schema)]

public class CountryInfo:BaseEntity<Guid>
{
    
    public CountryCode CountryCode {  get; set; }
    
    [Required]
    public string PhoneCode { get; set; } = null!;

    public ICollection<CountryInfoTranslation> Translations = new List<CountryInfoTranslation>();

}
