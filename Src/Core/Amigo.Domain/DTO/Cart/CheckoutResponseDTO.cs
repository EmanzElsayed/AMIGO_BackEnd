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
        bool? IsTourTitleChanged = false,
        bool? IsDestinationNameChanged = false
        
       );
}
