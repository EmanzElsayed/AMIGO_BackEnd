using Amigo.Domain.DTO.CountryInfo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Destination
{
    public record GetDestinationDetailsDTO
     (
        Guid DestinationId,
        CountryInfoDetailsDTO? Country,
        bool IsActive,
        string? ImageUrl,
         string? Name,
        string? Language,
        string? Description
       

    );
}
