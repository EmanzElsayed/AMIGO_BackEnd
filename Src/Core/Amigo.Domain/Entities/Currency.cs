using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities
{
    public class Currency:BaseEntity<Guid>
    {
        public CurrencyCode CurrencyCode { get; set; }
        
        public string? Icon { get; set; } 

        public ICollection<CurrencyTranslation> Translations = new List<CurrencyTranslation>();
    }
}
