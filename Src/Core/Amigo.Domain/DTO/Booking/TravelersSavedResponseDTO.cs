using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Booking
{
    public record TravelersSavedResponseDTO
     (
         Guid BookingId,
         int TravelersCount,
         bool Completed
     );
}
