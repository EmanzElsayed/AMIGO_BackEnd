using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Currency
{
   public record CurrencyRateBulkItemDTO(
    
    CurrencyCode BaseCurrency,
    CurrencyCode TargetCurrency,
    decimal Rate);

}
