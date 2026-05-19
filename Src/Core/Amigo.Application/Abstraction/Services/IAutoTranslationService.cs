using Amigo.Domain.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IAutoTranslationService
    {
        Task<Result> TranslateAllPendingTours(GetLanguageFromBodyDTO requestDto);

    }
}
