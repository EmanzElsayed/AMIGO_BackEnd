using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cart
{
   public record AddCartItemRequestDTO
    (
    
         Guid TourId ,
         Guid SlotId ,
       
         Language Language ,

         DateOnly TourDate ,
         TimeOnly StartTime,
         

         List<AddCartPriceRequestDTO> Prices 
    
     );
}
