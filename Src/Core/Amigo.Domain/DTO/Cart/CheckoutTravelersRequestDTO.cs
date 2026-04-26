using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cart
{
    public record CheckoutTravelersRequestDTO
    (
        string Type ,
        string FirstName,
        string LastName,
        string Nationality
        
    );
}
