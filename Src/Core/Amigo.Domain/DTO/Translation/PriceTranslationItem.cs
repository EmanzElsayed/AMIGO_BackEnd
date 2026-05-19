using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Translation
{
    public class PriceTranslationItem
    {
        public Guid PriceId { get; set; }
        public string Type { get; set; } = null!;
    }
}
