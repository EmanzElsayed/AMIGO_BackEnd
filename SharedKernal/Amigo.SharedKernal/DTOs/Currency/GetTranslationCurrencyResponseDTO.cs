using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Currency
{
    public record GetTranslationCurrencyResponseDTO
    (
        Guid TranslationId,
        string Name,
        string Language
    );
}
