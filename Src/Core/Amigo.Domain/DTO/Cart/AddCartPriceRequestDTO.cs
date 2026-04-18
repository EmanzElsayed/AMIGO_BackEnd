using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cart
{
    public record AddCartPriceRequestDTO
    (
        string Type ,
        int Quantity 
        
    );
}
