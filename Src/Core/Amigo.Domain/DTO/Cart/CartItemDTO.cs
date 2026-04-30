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
                                   
     Language Language            ,
     DateOnly TourDate            ,
     TimeOnly StartTime           ,
                                   
     string TourName              ,
     string DestinationName       ,
                                   
     decimal TotalAmount          ,
                                   
      List<CartPriceDTO>? Prices,
      List<CheckoutTravelersRequestDTO>? Travelers,
      string? PhoneCode = null,
      string? PhoneNumber = null,
      string? HotelNameAddress = null,
      string? CommentForProvider = null
   );
}
