using Amigo.Domain.DTO.Translation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface ITourTranslationQueryService
    {
        Task<List<TourTranslationItem>>
          GetPendingTranslationToursAsync(SupportedLanguage baseLanguage);
    }
}
