using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.CountryInfo
{
    public record CountryInfoDetailsDTO
    (
         Guid? CountryInfoId,
        string? CountryCode,
        string? PhoneCode,
        string? Name,
        string? Language,
        string? CountryImageUrl,
        string? CountryDescription
     );
}
