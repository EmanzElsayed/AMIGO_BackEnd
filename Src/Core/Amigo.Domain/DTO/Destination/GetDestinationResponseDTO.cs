using Amigo.Domain.DTO.CountryInfo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.DTOs.Destination
{
    public record GetDestinationResponseDTO
    (
        Guid DestinationId,
        GetCountryInfoResponseDTO Country,
        bool IsActive,
        string? ImageUrl,
         string? Name,
        string? Language
        
    );
}
