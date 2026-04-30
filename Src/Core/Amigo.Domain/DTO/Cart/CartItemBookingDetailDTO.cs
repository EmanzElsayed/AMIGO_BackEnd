using System;
using System.Collections.Generic;

namespace Amigo.Domain.DTO.Cart
{
    public record CartItemBookingDetailDTO
    (
        List<CheckoutTravelersRequestDTO> Travelers,
        string? FirstName = null,
        string? LastName = null,
        string? Email = null,
        string? PhoneCode = null,
        string? PhoneNumber = null,
        string? HotelNameAddress = null,
        string? CommentForProvider = null
    );
}
