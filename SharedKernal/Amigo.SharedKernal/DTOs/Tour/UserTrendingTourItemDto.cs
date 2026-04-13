namespace Amigo.SharedKernal.DTOs.Tour;

public record UserTrendingTourItemDto(
    Guid TourId,
    string Title,
    string? HeroImageUrl,
    decimal? AverageRating,
    int ReviewCount,
    string? FromPrice,
    string BaseCurrency,
    decimal? BaseAmount,
    string TourSlug,
    string DestinationSlug);
