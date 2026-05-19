using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Translation
{
    public class CancellationTranslationItem
    {
        public Guid CancellationId { get; set; }
        public string Description { get; set; } = null!;
    }
}
