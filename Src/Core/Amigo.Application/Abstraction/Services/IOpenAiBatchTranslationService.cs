using Amigo.Domain.DTO.Translation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IOpenAiBatchTranslationService
    {
        Task<List<ToursTranslationAiResult>> TranslateToursAsync( List<TourTranslationItem> tours);
        
        
        public Task<List<TourTranslationAiResult>> TranslateTourAsync(
         TourTranslationItem tour,
         SupportedLanguage sourceLanguage
         );


        public Task<List<DestinationTranslationAiResult>> TranslateDestinationAsync(
        DestinationTranslationItem destination,
        SupportedLanguage sourceLanguage
        );
        
    }
}
