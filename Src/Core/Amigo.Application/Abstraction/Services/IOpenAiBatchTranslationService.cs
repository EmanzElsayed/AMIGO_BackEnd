using Amigo.Domain.DTO.Translation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IOpenAiBatchTranslationService
    {
        Task<List<TourTranslationAiResult>> TranslateToursAsync( List<TourTranslationItem> tours);

    }
}
