using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cart
{
   public record CheckoutResponseDTO
    (
         Guid OrderId,
         Guid PaymentId,
      
        List<CheckoutPriceResponseDTO>? ChangedPrices ,
        string? CurrencyCode = null,
        bool? IsTourTitleChanged = false,
        bool? IsDestinationNameChanged = false
        
       );
}
