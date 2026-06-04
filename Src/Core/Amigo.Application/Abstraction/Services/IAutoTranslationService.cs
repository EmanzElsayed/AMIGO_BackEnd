using Amigo.Domain.DTO.Enums;
using Amigo.Domain.DTO.Translation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IAutoTranslationService
    {
        Task<Result> TranslateAllPendingTours(GetLanguageFromBodyDTO requestDto);
        Task<Result> TranslateTour(SupportedLanguage sourceLanguage, TourTranslationItem tourTranslationItem);
        
    }
}
