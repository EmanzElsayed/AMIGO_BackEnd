using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.CountryInfo
{
    public record CountryDescriptionQueryDTO
    (
        string CountryCode,
        string Language
    );
}
