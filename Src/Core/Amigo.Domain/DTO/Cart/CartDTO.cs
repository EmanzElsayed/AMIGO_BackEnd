using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cart
{
    public record CartDTO
    (
         Guid Id, 
         string? UserId, 
         string? CartToken, 
         CurrencyCode CurrencyCode, 
         decimal TotalAmount,
         int TotalItems,
         DateTime LastUpdatedAt, 
         DateTime ExpiresAt ,

         List<CartItemDTO>? Items     
        
    );
}
