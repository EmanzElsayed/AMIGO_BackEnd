using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cart
{
    public record UpdateCartItemRequestDTO
    (
        List<AddCartPriceRequestDTO> Prices,
        List<CheckoutTravelersRequestDTO>? Travelers
    );
}
