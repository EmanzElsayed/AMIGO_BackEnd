using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.CountryInfo
{
    public record GetCountryByIdResponseDTO
    (
        Guid CountryId,
        string? CountryCode,
        string? PhoneCode,
        string? Capital,
        string? OfficialLanguage,
        string? ImageUrl,
         string? Name,
         string? Language,
        int? ReviewsCount,
        int? TravelersCount,
        decimal? AverageReviewRating,
        int ToursCount,
        int DestinationCount,
        string? Description
    );
}
