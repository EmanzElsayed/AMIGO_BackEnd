using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Destination
{
    public record CreateDestinationRequestDTO
    (
        string? ImageUrl ,
        string? PublicId,
        bool? IsActive, 
        string CountryCode,
        string Name ,
        string Language
    );
}
