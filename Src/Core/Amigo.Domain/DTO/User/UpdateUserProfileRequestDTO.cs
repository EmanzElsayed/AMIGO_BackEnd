using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.User
{
    public record UpdateUserProfileRequestDTO
    (
        string? FullName,
        //string? Email,   for advance make for user multi email
        string? CountryIsoCode,

        string? PhoneNumber,
        string? ImageUrl,
        string? ImagePublicId,

        string? Gender,
        DateOnly? BirthDate,
        string? Nationality,
        string? Language,
        string? BuildingNumber,
        string? City,
        string? Country
    );
}
