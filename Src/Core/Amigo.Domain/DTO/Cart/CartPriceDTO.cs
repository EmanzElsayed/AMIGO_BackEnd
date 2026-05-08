using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cart
{
    public record CartPriceDTO
        (

            Guid Id,
            string Type,
            decimal ConvertedRetailPrice,
            int Quantity,
            decimal FinalPrice
        );
    
}
