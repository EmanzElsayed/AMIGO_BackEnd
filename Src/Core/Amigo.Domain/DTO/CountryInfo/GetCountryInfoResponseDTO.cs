using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.CountryInfo
{
    public record GetCountryInfoResponseDTO
    (
        Guid CountryInfoId,
        string CountryCode,
        string PhoneCode,
        string Name,
        string Language
    );
}
