using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cart
{
    public record CartPriceDTO
        (

            Guid Id,
            string Type,
            decimal RetailPrice,
            int Quantity,
            decimal FinalPrice
        );
    
}
