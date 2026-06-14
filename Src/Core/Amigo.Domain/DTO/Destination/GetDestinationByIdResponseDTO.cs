using Amigo.Domain.DTO.CountryInfo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Destination
{
    public record GetDestinationByIdResponseDTO
    (
        Guid DestinationId,
        GetCountryInfoResponseDTO? Country,
        bool IsActive,
        string? ImageUrl,
         string? Name,
         string? Language,
        int? ReviewsCount,
        int? TravelersCount,
        decimal? AverageReviewRating
    );
}
