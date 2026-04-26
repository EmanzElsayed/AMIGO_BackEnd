using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cart
{
    public record CheckoutItemRequestDTO
    (
        Guid CartItemId,
        List<CheckoutTravelersRequestDTO> Travelers,
        string? NameAndAddressAccommodation,
        string? CommentForProvider
        
    );
}
