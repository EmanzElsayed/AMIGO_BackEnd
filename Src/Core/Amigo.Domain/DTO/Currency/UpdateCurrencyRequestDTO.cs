using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Currency
{
    public record UpdateCurrencyRequestDTO
    (
         string? Icon,
         string? CurrencyCode,

         string? Name,
         string? Language
     );
}
