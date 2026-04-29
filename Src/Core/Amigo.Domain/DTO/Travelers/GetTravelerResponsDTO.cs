using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Travelers
{
    public record GetTravelerResponsDTO
    (
        Guid TravelerId,
        string FirstName,
        string LastName,
        string Nationality,
        string? PassportNumber,
        DateOnly? BirthDate

    );
}
