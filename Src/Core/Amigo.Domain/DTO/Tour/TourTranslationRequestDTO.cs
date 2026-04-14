using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Tour
{
    public class TourTranslationRequestDTO
    {
        public Guid TourId { get; set; }
        public Language SourceLanguage { get; set; }
    }
}
