using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Search
{
    public record SearchResponseDTO
    (
        string? DestinationName,
        string? DestinationSlug,
        string? CountryName,
        string? CountrySlug
        
    );
}
