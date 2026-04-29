using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Currency
{
    public record GetCurrencyResponseDTO
    (
        Guid CurrencyId,
        string CurrencyCode,
        string? Icon ,
        string?CodeIcon,
        string Name,
        string Language

    );
}
