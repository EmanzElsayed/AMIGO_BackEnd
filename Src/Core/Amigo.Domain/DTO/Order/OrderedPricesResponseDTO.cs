using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Order
{
    public record OrderedPricesResponseDTO
    (
        Guid PriceId,
        string Type,
        decimal RetailPrice,
        int Quantity,
        decimal FinalPrice
     );
}
