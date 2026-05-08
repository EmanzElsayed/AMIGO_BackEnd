using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Order
{
    public record OrderedPricesResponseDTO
    (
        Guid PriceId,
        string Type,
        decimal ConvertedRetailPrice,
        int Quantity,
        decimal FinalPrice
     );
}
