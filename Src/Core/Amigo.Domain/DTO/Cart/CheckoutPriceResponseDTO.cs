using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cart
{
    public record CheckoutPriceResponseDTO
    (
        string Type,
        decimal RetailPrice
        
    );
}
