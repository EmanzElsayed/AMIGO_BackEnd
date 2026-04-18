using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Booking
{
    public record AddTravelersRequestDTO
    (
        List<TravelerDTO> Travelers
    );
}
