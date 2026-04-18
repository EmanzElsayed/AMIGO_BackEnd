using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Booking
{
    public record TravelerDTO
     (
         string FullName,
         string Nationality,
         DateOnly? BirthDate,
         string? PassportNumber,
         string? PhoneNumber
     );
}
