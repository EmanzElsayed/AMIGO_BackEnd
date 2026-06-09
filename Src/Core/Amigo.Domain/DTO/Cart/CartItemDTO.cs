using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cart
{
   public record CartItemDTO
   (
     Guid Id               ,    
       
     Guid TourId                  ,
     Guid SlotId                  ,
                                   
     SupportedLanguage Language            ,
     DateOnly TourDate            ,
     TimeOnly StartTime           ,
     string? ImageUrl,                              
     string TourName              ,
     string DestinationName       ,
                                   
     decimal TotalAmount          ,
      string?ActivityType,                             
      List<CartPriceDTO>? Prices,
      List<CheckoutTravelersRequestDTO>? Travelers
   );
}
